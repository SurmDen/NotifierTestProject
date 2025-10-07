using Microsoft.AspNetCore.Mvc;
using NotifierTestProject.Entities;
using NotifierTestProject.Interfaces;
using NotifierTestProject.Models;

namespace NotifierTestProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotifierController : ControllerBase
    {
        private readonly INoticeRepository _noticeRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<NotifierController> _logger;

        public NotifierController(
            INoticeRepository noticeRepository,
            IUserRepository userRepository,
            ILogger<NotifierController> logger)
        {
            _noticeRepository = noticeRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpGet("notices")]
        public async Task<ActionResult<List<Notice>>> GetNotices()
        {
            try
            {
                var notices = await _noticeRepository.GetNoticesAsync();
                return Ok(notices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting notices");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("notices")]
        public async Task<ActionResult> CreateNotice([FromBody] CreateNoticeDTO createNoticeDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _noticeRepository.CreateNoticeAsync(createNoticeDTO);
                return Ok("Notice created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating notice");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("users")]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            try
            {
                var users = await _userRepository.GetUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting users");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("users/upload-csv")]
        public async Task<ActionResult> UploadUsersFromCsv(IFormFile csvFile)
        {
            if (csvFile == null || csvFile.Length == 0)
            {
                return BadRequest("CSV file is required");
            }

            if (!csvFile.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only CSV files are supported");
            }

            try
            {
                var csvUsers = await ParseCsvFile(csvFile);
                await _userRepository.LoadUsersAsync(csvUsers);

                return Ok($"Successfully uploaded {csvUsers.Count} users");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while uploading users from CSV");
                return StatusCode(500, "Error processing CSV file");
            }
        }

        [HttpPost("notify/{noticeId:guid}")]
        public async Task<ActionResult> NotifyUsers(Guid noticeId)
        {
            try
            {
                await _userRepository.NotifyUsersAsync(noticeId);
                return Ok("Users notified successfully");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Notice not found");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while notifying users");
                return StatusCode(500, "Internal server error");
            }
        }

        private async Task<List<CsvUser>> ParseCsvFile(IFormFile file)
        {
            var csvUsers = new List<CsvUser>();

            using var stream = new StreamReader(file.OpenReadStream());

            await stream.ReadLineAsync();

            string line;
            while ((line = await stream.ReadLineAsync()) != null)
            {
                var values = line.Split(',');

                if (values.Length >= 2 &&
                    long.TryParse(values[0], out long userNumber))
                {
                    csvUsers.Add(new CsvUser
                    {
                        UserNumber = userNumber,
                        UserName = values[1].Trim()
                    });
                }
            }

            return csvUsers;
        }
    }
}
