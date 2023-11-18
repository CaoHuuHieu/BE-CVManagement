using CVManagement.Exceptions;
using CVManagement.Models.DataTransferObject;
using CVManagement.Models.Entities;
using CVManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CVManagement.Web.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        private readonly IMailService _mailService;
        public AuthController(ITokenService tokenService, IUserService userService, IMailService mailService)
        {
            _userService = userService;
            _tokenService = tokenService;
            _mailService = mailService;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin account)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.FindByEmail(account.Email);
                if (user != null && user.Status != (int)UserStatus.Lock )
                {
                    bool IsValidPass = _userService.CheckPassword(account.Password, user);
                    if (IsValidPass)
                    {
                        JWTTokenResponse tokenReponse = _tokenService.GetToken(user);
                        user.Token = tokenReponse.Token;
                        await _userService.Update(user);
                        return Ok(tokenReponse);
                    }
                }
            }
            return BadRequest(new { Message = "Your username or password is invalid!" });
        }
   /*     [Authorize(Roles = "HrManager")]*/
        [HttpPost("register")]
        public  async Task<IActionResult> RegisterAccountAsync(Register newuser)
        {
            try
            {
                var user = await _userService.FindByEmail(newuser.Email);
                if (user == null)
                {
                    await _userService.Save(newuser);
                    return Ok(new { Message = "Sign Up Success!" });
                }
                return BadRequest(new { Message = "This email existed, please use another one." });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }
        [HttpPost("otp/send")]
        public async Task<IActionResult> SendOTP(string email)
        {
            try
            {
                await _mailService.SendOTPAsync(email);
                return Ok(new { Message = "Send otp successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return BadRequest(new { Message = "Recerver is not valid" });
            }
        }
        [HttpPost("otp/validate")]
        public async Task<IActionResult> ValidateOTP([FromBody] OTPResetPassword otp)
        {
            if (!ModelState.IsValid)
                return BadRequest(new {Message = "Email or otp is not valid!"});
            else
            {
                try
                {
                    bool isValidOtp = await _userService.ValidateOTP(otp);
                    if (isValidOtp == true)
                    {
                        return Ok(new {Message = "OTP is valid"});
                    }
                    else
                        return BadRequest(new { Message = "OTP is not valid" });
                }
                catch (EntityException e)
                {
                    return BadRequest(new {Message = "Email is not valid" });
                }
            }
        }
        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordReset passwordReset)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { Message = "Email or password is not valid!" });
                else {
                    bool isReseted = await _userService.ResetPassword(passwordReset);
                    if (isReseted == true)
                    {
                        return Ok(new {Message = "Reset password successfully"});
                    }
                    else
                        return BadRequest(new { Message = "Opp! Password doesn't reset" });
                }
            }
            catch (EntityException e)
            {
                return BadRequest(new {Message =  "Email is not valid!" });
            }
        }
    }
}
