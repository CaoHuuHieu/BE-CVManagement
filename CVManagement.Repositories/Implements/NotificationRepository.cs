using CVManagement.Models.Entities;
using CVManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CVManagement.Repositories.Implements
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly DataContext _dbContext;
        public NotificationRepository(DataContext dBContext)
        {
            _dbContext = dBContext;
        }
        public async Task<bool> AddNotification(Notification notification)
        {
            if(notification == null) 
                throw new ArgumentNullException("Notification is null");
            else
            {
                _dbContext.Notifications.Add(notification);
                return await _dbContext.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> AddNotification(ICollection<Notification> notifications)
        {
            if (notifications == null) 
                throw new ArgumentNullException("Notification is null");
            else
            {
                _dbContext.Notifications.AddRange(notifications);
                return await _dbContext.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> DeleteNotification(long id)
        {
            var notification = _dbContext.Notifications.FirstOrDefault(x => x.Id == id);
            if (notification == null)
                throw new ArgumentNullException("Notification is null");
            else
            {
                _dbContext.Notifications.Remove(notification);
                return await _dbContext.SaveChangesAsync() > 0;
            }
        }

        public async Task<IEnumerable<Notification>> GetNotifications()
        {
           return await _dbContext.Notifications.OrderByDescending(x => x.CreateDate).ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetNotifications(long userId)
        {
            return await _dbContext.Notifications.Where(n => n.UserId == userId).OrderByDescending(x => x.CreateDate).ToListAsync();
        }

        public async Task<bool> Update(Notification notification)
        { 
            if (notification == null)
                throw new ArgumentNullException("Notification is null");
            else
            {
                _dbContext.Notifications.Update(notification);
                 return await _dbContext.SaveChangesAsync() > 0;
            }
        }
    }
}
