using CVManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CVManagement.Web.Controllers
{
    [Route("api")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        [Authorize(Roles = "HrManager, Hr, Customer")]
        [HttpGet("notification")]
        public async Task<IActionResult> GetNotifcation()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var notifications = await _notificationService.GetNotifications(long.Parse(userId));
            return Ok(notifications);
        }

        [Authorize(Roles = "HrManager, Hr, Customer")]
        [HttpGet("notification/unread")]
        public async Task<IActionResult> GetUnreadNotifcation()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var notifications = await _notificationService.GetUnreadNotifcation(long.Parse(userId));
            return Ok(notifications);
        }
        [Authorize(Roles = "HrManager, Hr, Customer")]
        [HttpPost("notification/mask-as-read")]
        public async Task<IActionResult> MaskAsReadNotifcation()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _notificationService.MaskAsRead(long.Parse(userId));
            return Ok();
        }
    }
}
