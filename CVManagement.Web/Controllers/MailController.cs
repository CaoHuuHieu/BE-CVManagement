using CVManagement.Exceptions;
using CVManagement.Models.DataTransferObject;
using CVManagement.Models.Entities;
using CVManagement.Services.Interfaces;
using CVManagement.Web.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CVManagement.Controllers
{
    [Route("api/mail")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly IHubContext<NotificationHub, INotificationHub> _hubContext;
        private readonly IMailService _mailService;
        private readonly INotificationService _notificationService;
        public MailController(IMailService mailService, INotificationService notificationService, IHubContext<NotificationHub, INotificationHub> hubContext )
        {
            _mailService = mailService;
            _notificationService = notificationService;
            _hubContext = hubContext;
        }
       
        [HttpPost("share")]
        [Authorize(Roles = "HrManager, Hr")]
        public async Task<IActionResult> ShareCVAsync([FromBody] MailRequest mail)
        {
            var hrId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {
                bool isShared = await _mailService.StoreAndSendCV(long.Parse(hrId), mail);
                await _notificationService.CreateNotificationShareCV(long.Parse(hrId), mail);
                await _hubContext.Clients.All.SendNotification("Hr vừa share cv");
                return Ok(new { Message = "Shared successfully" });
            }
            catch (EntityException ex)
            {
                Console.WriteLine(ex.ToString());
                return BadRequest(new { Message = "CV or Receiver is not valid!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(500);
            }
        }
        [HttpPost("resend")]
        [Authorize(Roles = "HrManager, Hr")]
        public async Task<IActionResult> SendReminderEmail(long[] customerId)
        {
            var hrId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {
                await _mailService.SendReminderEmail(long.Parse(hrId), customerId);
                return Ok(new { Message = "Resend successfully" });
            }
            catch (EntityException ex)
            {
                Console.WriteLine(ex.ToString());
                return BadRequest(new { Message = "CV or Receiver is not valid!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(500, new {Message = "Internal server error"});
            }
        }
    }
}
