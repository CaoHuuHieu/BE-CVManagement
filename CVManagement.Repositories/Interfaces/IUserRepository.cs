using CVManagement.Models.DataTransferObject;
using CVManagement.Models.Entities;

namespace CVManagement.Repositories.Interfaces
{
    public interface IUserRepository
    {
        public Task<IEnumerable<User>> GetAll();
        public Task<User> GetById(long id);
        public Task<bool> Update(User user);
        public Task<bool> Save(User user);
        public Task<bool> Delete(User user);
        public Task<User> FindByEmail(string email);
        public Task<ICollection<UserWithUnreadCV>> GetUserWithUnreadCV();
        public Task<ICollection<UserWithUnreadCV>> GetUserWithUnreadCV(long senderId, long[] customerIds);
        Task<User> FindByRole(string hrmanager);
    }
}
