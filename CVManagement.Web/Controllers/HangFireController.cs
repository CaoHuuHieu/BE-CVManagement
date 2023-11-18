using CVManagement.Models.DataTransferObject;
using CVManagement.Models.Entities;
using CVManagement.Repositories.Interfaces;
using CVManagement.Services.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace CVManagement.Web.Controllers
{
    [Route("api/")]
    [ApiController]
    public class HangFireController : ControllerBase
    {
        private readonly IMailService _mailService;
        private readonly INotificationService _notificationRepos;
        public HangFireController(IMailService mailService, INotificationService notificationRepos, IHostingEnvironment env)
        {
            _mailService = mailService;
            _notificationRepos = notificationRepos;
        }
        [HttpPost("active-resend-cv")]
        public void ActiveResendCV()
        {
            RecurringJob.AddOrUpdate("send-email", () => _mailService.ResendCV(), "2 0/2 * ? * *");
        }

        [HttpPost("active-notification")]
        public void ActiveNotification()
        {
            RecurringJob.AddOrUpdate("notify-unreadcv", () => _notificationRepos.CreateNotificationReminderCustomerReadCV(), "2 0/2 * ? * *");
        }
    }
}
