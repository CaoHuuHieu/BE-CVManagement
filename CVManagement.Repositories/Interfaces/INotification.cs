using CVManagement.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVManagement.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task<IEnumerable<Notification>> GetNotifications();
        Task<IEnumerable<Notification>> GetNotifications(long userId);
        Task<bool> AddNotification(Notification notification);
        Task<bool> AddNotification(ICollection<Notification> notification);
        Task<bool> DeleteNotification(long id);
        Task<bool> Update(Notification notification);
    }
}
