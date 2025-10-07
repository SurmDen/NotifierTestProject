using NotifierTestProject.Entities;
using NotifierTestProject.Models;

namespace NotifierTestProject.Interfaces
{
    public interface IUserRepository
    {
        public Task LoadUsersAsync(List<CsvUser> csvUsers);

        public Task<List<User>> GetUsersAsync();

        public Task NotifyUsersAsync(Guid noticeId);
    }
}
