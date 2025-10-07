using Microsoft.EntityFrameworkCore;
using NotifierTestProject.Data;
using NotifierTestProject.Entities;
using NotifierTestProject.Interfaces;
using NotifierTestProject.Models;

namespace NotifierTestProject.Services
{
    public class UserRepository : IUserRepository
    {
        public UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserRepository> _logger;
        public async Task<List<User>> GetUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Notices)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task LoadUsersAsync(List<CsvUser> csvUsers)
        {
            if (csvUsers == null)
            {
                _logger.LogWarning("scvUsers was null");

                throw new ArgumentNullException("scvUsers was null");
            }

            if (csvUsers.Count == 0)
            {
                _logger.LogWarning("scvUsers list was empty");

                throw new ArgumentException("scvUsers list was empty");
            }

            try
            {
                foreach (var csvUser in csvUsers)
                {
                    User user = new User()
                    {
                        UserName = csvUser.UserName,
                        UserNumber = csvUser.UserNumber,
                        Id = Guid.NewGuid()
                    };

                    await _context.Users.AddAsync(user);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occured while adding users to db");

                throw;
            }
            finally
            {
                await _context.SaveChangesAsync();
            }
        }

        public async Task NotifyUsersAsync(Guid noticeId)
        {
            Notice? notice = await _context.Notices.FirstOrDefaultAsync();

            if (notice == null)
            {
                _logger.LogWarning($"Notice with id {noticeId} was null");

                throw new ArgumentException($"Notice with id {noticeId} was null");
            }

            try
            {
                IEnumerable<User> users = _context.Users.Include(u => u.Notices);

                foreach (var user in users)
                {
                    // tracking, have the user notice with noticeId
                    if (user.Notices.FirstOrDefault(n => n.Id == noticeId) == null)
                    {
                        user.Notices.Add(notice);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occured while notify users to db");

                throw;
            }
            finally 
            {
                await _context.SaveChangesAsync(); 
            }
        }
    }
}
