using CVManagement.Exceptions;
using CVManagement.Models.DataTransferObject;
using CVManagement.Models.Entities;
using CVManagement.Services.Interfaces;
using CVManagement.Web.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace CVManagement.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IHubContext<NotificationHub, INotificationHub> _hubContext;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;
        public AccountController(IUserService userService,  IHubContext<NotificationHub, INotificationHub> notificationHub, INotificationService notificationService)
        {
            _userService = userService;
            _hubContext = notificationHub;
            _notificationService = notificationService;
        }
        [HttpGet()]
        [Authorize(Roles = "HrManager, Hr")]
        public async Task<IActionResult> GetAllUser()
        {
            var userid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            ICollection<UserBasicInfor> users;
            if (role.Equals("HrManager"))
            {
                users = await _userService.GetAllCustomerForHrManager();
            }
            else
                users = await _userService.GetAllCustomersByHrId(long.Parse(userid));
            return Ok(users);
        }
        [HttpGet("hr")]
        [Authorize(Roles = "HrManager")]
        public async Task<IActionResult> GetAllHumamResource()
        {
            var humanResources = await _userService.GetAllHumanResource();
             return Ok(humanResources);
        }
        [HttpGet("valid-hr")]
        [Authorize(Roles = "HrManager")]
        public async Task<IActionResult> GetAllValidHumamResource()
        {
            var humanResources = await _userService.GetAllValidHumanResource();
             return Ok(humanResources);
        }
        [HttpGet("customer")]
        [Authorize(Roles = "HrManager")]
        public async Task<IActionResult> GetAllValidCustomer()
        {
            var customers = await _userService.GetAllCustomerForAssignToHr();
            return Ok(customers);
        }

        [HttpGet("hr/{hrId}")]
        [Authorize(Roles = "HrManager")]
        public async Task<IActionResult> ViewDetailOfHumanResource(long hrId)
        {
            var hrDatail = await _userService.GetDetailOfHumanResource(hrId);
            return Ok(hrDatail);
        }

        [HttpGet("{id}")]
        [Authorize(Roles ="HrManager, Hr, Customer")]
        public async Task<IActionResult> GetUserById(int id) 
        {
            try
            {
                var user =await _userService.GetById(id);
                return Ok(user);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return NotFound(new { Message = "User not found" });
            }
        }

       

        [HttpPost("lock-unlock/{id}")]
        [Authorize(Roles = "HrManager")]
        public async Task<IActionResult> LockAccount(int id)
        {
            try
            {
                bool isDelete = await _userService.LockOrUnlockAccount(id);
                return Ok(new { Message = "Successfully" });
            }
            catch (EntityException e)
            {
                Console.WriteLine(e.Message);
                return BadRequest(new { Message = "User is not exists!" });
            }
        }
        

        [HttpPost("assign")]
        [Authorize(Roles = "HrManager")]
        public async Task<IActionResult> AssignCustomerToHumanResource(HumanResourceAndCustomers hrAndCustomers)
        {
            try
            {
                await _userService.AssignCustomerToHumanResource(hrAndCustomers);
                await _notificationService.CreateNotificationAssignCustomer(hrAndCustomers);
                await _hubContext.Clients.All.SendNotification("HrManager vừa gắn khách hàng");
                return Ok(new { Message = "Assign Successfully" });
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(500, new { Message = "Internal server error" });
            }
        }

        [HttpDelete("delete-customer/{hrId}/{customerId}")]
        [Authorize(Roles = "HrManager, Hr")]
        public async Task<IActionResult> DeleteCustomerOfHumanResource(long hrId, long customerId)
        {
            try
            {
                await _userService.DeleteCustomerOfHumanResource(hrId, customerId);
                await _notificationService.CreateNotificationDeleteCustomerOfHr(hrId, customerId);
                await _hubContext.Clients.All.SendNotification("HrManager vừa gỡ khách hàng");
                return Ok(new { Message = "Delete Successfully" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(500, new { Message = "Internal server error" });
            }
        }

    }
}
