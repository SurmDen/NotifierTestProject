using NotifierTestProject.Entities;
using NotifierTestProject.Models;

namespace NotifierTestProject.Interfaces
{
    public interface INoticeRepository
    {
        public Task<List<Notice>> GetNoticesAsync();

        public Task CreateNoticeAsync(CreateNoticeDTO createNoticeDTO);
    }
}
