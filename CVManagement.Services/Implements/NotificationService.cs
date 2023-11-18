using CVManagement.Models.DataTransferObject;
using CVManagement.Models.Entities;
using CVManagement.Repositories.Interfaces;
using CVManagement.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;

namespace CVManagement.Services.Implements
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepos;
        private readonly IUserRepository _userRepository;
        private readonly ICVRepository _CVRepository;
        private readonly IConfiguration _config;
        public NotificationService(INotificationRepository notificationRepos, IUserRepository userRepository, ICVRepository cVRepository, IConfiguration config)
        {
            _notificationRepos = notificationRepos;
            _userRepository = userRepository;
            _CVRepository = cVRepository;
            _config = config;
        }


        public async Task<bool> AddNotification(Notification notification)
        {
            return await _notificationRepos.AddNotification(notification) ;
        }

        public async Task<bool> AddNotification(ICollection<Notification> notifications)
        {
            return await _notificationRepos.AddNotification(notifications);
        }

        public async Task<bool> DeleteNotification(long id)
        {
            return await _notificationRepos.DeleteNotification(id);
        }

        public async Task<IEnumerable<Notification>> GetNotifications()
        {
            return await _notificationRepos.GetNotifications();
        }

        public async Task<IEnumerable<Notification>> GetNotifications(long userId)
        {
            return await _notificationRepos.GetNotifications(userId);
        }
        public async Task<IEnumerable<Notification>> GetUnreadNotifcation(long userId)
        {
            IEnumerable<Notification> notifications = await _notificationRepos.GetNotifications(userId);
            return notifications.Where(n => n.Status == NotificationStatus.Unread);
        }
        public async Task<bool> MaskAsRead(long userId)
        {
            IEnumerable<Notification> notifications = await _notificationRepos.GetNotifications(userId);
            foreach (var notification in notifications)
            {
                notification.Status = NotificationStatus.Read;
                await _notificationRepos.Update(notification);
            }
            return true;
        }

        public async Task<bool> CreateNotificationShareCV(long hrId, MailRequest mail)
        {
            var hr = await _userRepository.GetById(hrId);
            var hrmanager = await _userRepository.FindByRole(UserRole.Hrmanager);
            ICollection<Notification> notifications = new List<Notification>();
            foreach (var userId in mail.UserIds)
            {
                var user  = await _userRepository.GetById(userId);
                notifications.Add(new Notification
                {
                    Message = $"{hr.FullName} sent file to customer {user.FullName}.",
                    Link = $"{_config["Link:HrViewCustomerCV"]}{user.Id}",
                    User = hrmanager
                });
                foreach(var cvId in mail.CVIds)
                {
                    var cv = await _CVRepository.GetById(cvId);
                    notifications.Add(new Notification
                    {
                        Message = $"{hr.FullName} sent you a file {cv.FileName}.",
                        Link = $"{_config["Link:ViewFile"]}{cv.Id}",
                        User = user,
                    });
                }
            }
            return await _notificationRepos.AddNotification(notifications);
        }

        public async Task CreateNotificationUploadCV(long hrId, int cv)
        {
            var hr = await _userRepository.GetById(hrId);
            var hrmanager = await _userRepository.FindByRole(UserRole.Hrmanager);
            Notification notification = new Notification
            {
                Message = $"{hr.FullName} uploaded {cv} CVs.",
                Link = $"{_config["Link:ViewHrCV"]}{hr.Id}",
                User = hrmanager,
            };
            await _notificationRepos.AddNotification(notification);
        }

        public async Task CreateNotificationAssignCustomer(HumanResourceAndCustomers hrAndCustomers)
        {
            int customers = hrAndCustomers.CustomerIds.Length;
            foreach (var hrId in  hrAndCustomers.HrIds) {
                Notification notification = new Notification
                {
                    Message = $"HrManager has assigned you {customers} customer.",
                    Link = $"{_config["Link:HrCustomer"]}",
                    UserId = hrId,
                };
                await _notificationRepos.AddNotification(notification);
            }
          
        }

        public async Task CreateNotificationDeleteCustomerOfHr(long hrId, long customerId)
        {
            var customer = await _userRepository.GetById(customerId);
            Notification notification = new Notification
            {
                Message = $"HrManager has deleted your customer {customer.FullName}.",
                Link = $"{_config["Link:HrCustomer"]}",
                UserId = hrId,
            };
            await _notificationRepos.AddNotification(notification);
        }
        public async Task CreateNotificationCustomerViewCV(long cvId, long customerId)
        {
            var cv = await _CVRepository.GetById(cvId);
            var customer = await _userRepository.GetById(customerId);
            Notification notification = new Notification
            {
                Message = $"{customer.FullName} has view {cv.FileName}.",
                Link = $"{_config["Link:HrViewCustomerCV"]}{customerId}",
                UserId = cv.PosterId,
            };
            await _notificationRepos.AddNotification(notification);
        }

        public async Task CreateNotificationDeleteCvOfHr(CV cv)
        {
            Notification notification = new Notification
            {
                Message = $"HrManager has deleted your file {cv.FileName}.",
                Link = $"{_config["Link:HrCustomer"]}",
                UserId = cv.PosterId,
            };
            await _notificationRepos.AddNotification(notification);
        }

        public async Task CreateNotificationReminderCustomerReadCV()
        {
            ICollection<UserWithUnreadCV> users = await _userRepository.GetUserWithUnreadCV();
            ICollection<Notification> notifications = new List<Notification>();
            foreach (var user in users)
            {
             
                foreach(var cv in user.UnreadCV)
                {
                    notifications.Add(new Notification
                    {
                        Message = $"You have not viewed file {cv.CV.FileName}.",
                        Link = $"{_config["Link:ViewFile"]}{cv.CV.Id}",
                        UserId = user.User.Id,
                    });
                    notifications.Add(new Notification
                    {
                        Message = $"{user.User.FullName} have not viewed file {cv.CV.FileName}.",
                        Link = $"{_config["Link:HrViewCustomerCV"]}",
                        UserId = cv.SenderId,
                    });
                }
                
            }
        }
    }
}