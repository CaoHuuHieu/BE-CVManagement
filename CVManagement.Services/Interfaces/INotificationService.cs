using CVManagement.Models.DataTransferObject;
using CVManagement.Models.Entities;


namespace CVManagement.Services.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<Notification>> GetNotifications();
        Task<IEnumerable<Notification>> GetNotifications(long userId);
        Task<IEnumerable<Notification>> GetUnreadNotifcation(long userId);
        Task<bool> AddNotification(Notification notification);
        Task<bool> AddNotification(ICollection<Notification> notification);
        Task<bool> DeleteNotification(long id);
        Task<bool> MaskAsRead(long userId);
        Task<bool> CreateNotificationShareCV(long hrId, MailRequest mail);
        Task CreateNotificationUploadCV(long hrId, int length);
        Task CreateNotificationAssignCustomer(HumanResourceAndCustomers hrAndCustomers);
        Task CreateNotificationDeleteCustomerOfHr(long hrId, long customerId);
        Task CreateNotificationDeleteCvOfHr(CV cv);
        Task CreateNotificationReminderCustomerReadCV();
    }
}
