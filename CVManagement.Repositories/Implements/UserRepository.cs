using CVManagement.Exceptions;
using CVManagement.Models.DataTransferObject;
using CVManagement.Models.Entities;
using CVManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CVManagement.Repositories.Implements
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _dbContext;
        public UserRepository(DataContext dataContext)
        {
            _dbContext = dataContext;
        }
        public async Task<bool> Delete(User user)
        {
            User userEntity = _dbContext.Users.Find(user.Id);
            if (userEntity == null)
            {
                _dbContext.Users.Remove(userEntity);
                return  await _dbContext.SaveChangesAsync() > 0;
            }
            else
                throw new EntityException("User is not found!");
        }

        public async Task<User> FindByEmail(string email)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<User> FindByRole(string hrmanager)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Role == hrmanager);
        }
        public async Task<IEnumerable<User>> GetAll()
        {
            return await _dbContext.Users.Include(u => u.Customers).Include(u => u.CVs).ToListAsync();
        }
     

        public async Task<User> GetById(long id)
        {
            return await _dbContext.Users.Include(u => u.Customers).Include(u => u.CVs).FirstOrDefaultAsync(u => u.Id == id);

        }

        public  async Task<bool> Save(User user)
        {
            if (user != null)
            {
                await _dbContext.Users.AddAsync(user);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            else
                throw new EntityException("User is not valid!");
         
        }

        public async Task<bool> Update(User user)
        {
            var userEntity = await _dbContext.Users.FindAsync(user.Id);
            if (userEntity != null)
            {
                _dbContext.Users.Update(user);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            throw new EntityException("User is not found!");
        }
      

        public async Task<ICollection<UserWithUnreadCV>> GetUserWithUnreadCV()
        {
            var users = await _dbContext.Users
                .Include(u => u.ReceiveCVs)
                .ToListAsync();
            return users
                .Where(u => u.Role == "Customer" && 
                u.ReceiveCVs.Any(cv => cv.Views == 0 && ((int)(cv.SendDate.AddMinutes(cv.ReminderInterval) - DateTime.Now).TotalMinutes) == 0))
              .Select(u => new UserWithUnreadCV
              {
                  User = new UserBasicInfor
                  {
                      Id = u.Id,
                      FullName = u.FullName,
                      Email = u.Email
                  },
                  UnreadCV = u.ReceiveCVs.Where(cv => cv.Views == 0
                     && ((int)(cv.SendDate.AddMinutes(cv.ReminderInterval) - DateTime.Now).TotalMinutes) == 0)
                .ToList()
              }).ToList();
        }
        public async Task<ICollection<UserWithUnreadCV>> GetUserWithUnreadCV(long senderId, long[] customerIds)
        {
            var users = await _dbContext.Users
                .Where(u => customerIds.Contains(u.Id))  
                .Include(u => u.ReceiveCVs)
                .ToListAsync();

            return users
                .Where(u => u.Role == "Customer" && u.ReceiveCVs.Any(cv => cv.Views == 0))
              .Select(u => new UserWithUnreadCV
              {
                  User = new UserBasicInfor
                  {
                      Id = u.Id,
                      FullName = u.FullName,
                      Email = u.Email
                  },
                  UnreadCV = u.ReceiveCVs.Where(cv => cv.Views == 0 && cv.SenderId == senderId)
                .ToList()
              }).ToList();
        }

    }
}
 
