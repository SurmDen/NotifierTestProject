using Microsoft.EntityFrameworkCore;
using NotifierTestProject.Data;
using NotifierTestProject.Entities;
using NotifierTestProject.Interfaces;
using NotifierTestProject.Models;

namespace NotifierTestProject.Services
{
    public class NoticeRepository : INoticeRepository
    {
        public NoticeRepository(ApplicationDbContext context, ILogger<NoticeRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        private readonly ApplicationDbContext _context;
        private readonly ILogger<NoticeRepository> _logger;

        public async Task<List<Notice>> GetNoticesAsync()
        {
            return await _context.Notices
                .Include(n => n.Users)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task CreateNoticeAsync(CreateNoticeDTO createNoticeDTO)
        {
            if (createNoticeDTO == null)
            {
                _logger.LogWarning("createNoticeDTO was null");

                throw new ArgumentNullException("scvUsers was null");
            }

            Notice notice = new Notice()
            {
                NotifierName = createNoticeDTO.NotifierName,
                Message = createNoticeDTO.Message,
                Id = Guid.NewGuid()
            };

            await _context.Notices.AddAsync(notice);
            await _context.SaveChangesAsync();
        }
    }
}
