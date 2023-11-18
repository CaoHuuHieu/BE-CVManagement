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
    [Route("api/cv")]
    [ApiController]
    public class CVController : ControllerBase
    {
        private readonly ICVService _cvService;
        private readonly IUserCVService _userCVService;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<NotificationHub, INotificationHub> _hubContext;
        public CVController(ICVService cvService, IUserCVService userCVService, INotificationService notificationService, IHubContext<NotificationHub, INotificationHub> hubContext)
        {
            _cvService = cvService;
            _userCVService = userCVService;
            _notificationService = notificationService;
            _hubContext = hubContext;
        }

        [HttpGet]
        [Authorize(Roles = "HrManager, Hr, Customer")]
        public async Task<IActionResult> GetCurriculumVitae()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            try
            {
                IEnumerable<CurriculumVitaeBasicInfor> curriculumVitaes;
                if (role.Equals("HrManager"))
                {
                    curriculumVitaes = await _cvService.GetAllCurriculumVitaes();
                }
                else if(role.Equals("Hr"))
                {
                    curriculumVitaes = await _cvService.GetAllCurriculumVitaes(long.Parse(userId));
                }
                else
                {
                    curriculumVitaes = await _cvService.GetCurriculumVitaesByCustomerId(long.Parse(userId));
                }
                return Ok(curriculumVitaes);
            }
            catch (Exception e)
            {
                return StatusCode(500, new
                {
                    Message = "An Internal Server Error!"
                });
            }
        }
        [HttpGet("{id}")]
        [Authorize(Roles = "HrManager, Hr, Customer")]
        public async Task<IActionResult> GetCurriculumVitaeById([FromRoute] long id)
        {
        
            try
            {
                CurriculumVitaeBasicInfor curriculumVitae =await _cvService.GetCurriculumVitaeById(id);
                return Ok(curriculumVitae);
            }
            catch (Exception e)
            {
                return StatusCode(500, new
                {
                    Message = "An Internal Server Error!"
                });
            }
        }
        [HttpGet("customer/{customerId}")]
        [Authorize(Roles = "HrManager, Hr")]
        public async Task<IActionResult> GetCurriculumVitaeOfCustomer(long customerId)
        {
            try
            {
             
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                IEnumerable<CurriculumVitaeForCustomer> curriculumVitaes;
                curriculumVitaes = await _cvService.GetCurriculumVitaeDetail(customerId);
                if (role.Equals(UserRole.Hr))
                {
                    var hrId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    curriculumVitaes = await _cvService.GetCurriculumVitaeDetail(long.Parse(hrId), customerId);
                }
                return Ok(curriculumVitaes);
            }
            catch (Exception e)
            {
                return StatusCode(500, new
                {
                    Message = "An Internal Server Error!!!"
                });
            }
        }
        [HttpGet("view/{id}")]
        [Authorize(Roles = "HrManager, Hr, Customer")]
        public async Task<IActionResult> ViewCurriculumVitae(long id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                if (role.Equals(UserRole.Customer))
                {
                    var checkPermission = await _userCVService.CheckPermission(id, long.Parse(userId));
                    if (!checkPermission)
                    {
                        return Forbid();
                    }
                }
                var cv = await _cvService.ViewById(id);
                return File(cv.FileByte, cv.ContentType);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(500, new
                {
                    Message = "An Internal Server Error!!!"
                });
            }
        }
        [HttpPut("rename/{id}")]
        [Authorize(Roles = "HrManager, Hr")]
        public async Task<IActionResult> RenameFile([FromRoute] long id, [FromBody] string newName)
        {
            try
            {
                await _cvService.Rename(id, newName);
                return Ok(new { Message = "ReName Succesfully" });
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return BadRequest(new { Message = "Not Found CV!" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(500, new { Message = "An internal server error occurred." });
            }
        }
        [HttpPost]
        [Authorize(Roles = "HrManager, Hr")]
        public async Task<IActionResult> UploadCurriculumVitae([FromForm] IFormFile[] file)
        {
            try
            {
                var hrId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                bool isSave = await _cvService.Save(long.Parse(hrId), file);
                if(!role.Equals(UserRole.Hrmanager)) {
                    await _notificationService.CreateNotificationUploadCV(long.Parse(hrId), file.Length);
                    await _hubContext.Clients.All.SendNotification("Hr vừa upload cv");
                }
                return Ok(new
                {
                    Message = "Upload Successfully!",
                });
            }
            catch (Exception e)
            {
                return StatusCode(500, new
                {
                    Message = "An Internal Server Error!!!",
                });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "HrManager, Hr")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {   
                CV cv = await _cvService.Delete(id);
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                if (role.Equals(UserRole.Hrmanager))
                {
                    await _notificationService.CreateNotificationDeleteCvOfHr(cv);
                    await _hubContext.Clients.All.SendNotification("Hr vừa upload cv");
                }
                return Ok(new { Message = "Delete CV Succesfully" });
            }
            catch(EntityException e)
            {
                return BadRequest(new { Message = "CV is invalid to delete." });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(500, new
                {
                    Message = "An Internal Server Error!!!"
                });
            }
        }
        [HttpDelete("delete-usercv/{cvId}/{customerId}")]
        [Authorize(Roles = "HrManager, Hr")]
        public async Task<IActionResult> DeleteCurriculumVitaeOfCustomer(long cvId, long customerId)
        {
            try
            {
                await _cvService.DeleteCurriculumVitaeOfCustomer(cvId, customerId);

                return Ok(new { Message = "Delete CV Succesfully" });
            }
            catch (EntityException e)
            {
                return BadRequest(new { Message = "UserCV is invalid to delete." });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(500, new
                {
                    Message = "An Internal Server Error!!!"
                });
            }
        }
    }
}
