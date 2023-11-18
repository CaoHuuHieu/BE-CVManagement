using CVManagement.Models.DataTransferObject;
using CVManagement.Models.Entities;
using CVManagement.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CVManagement.Services.Implements
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TokenService(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _config = config;
            _httpContextAccessor = httpContextAccessor;
        }
        
        public string GetReFreshToken()
        {

            throw new NotImplementedException();
        }

        public JWTTokenResponse GetToken(User user)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            List<Claim> claim = new List<Claim>(){
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };
            var tokeOptions = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"], 
                audience: _config["JWT:Audience"], 
                claims: claim, 
                expires: DateTime.Now.AddMinutes(30), 
                signingCredentials: signinCredentials
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            return new JWTTokenResponse()
            {
                Token = tokenString, 
                Email = user.Email,
                Role = user.Role,
                Status = user.Status,
                FullName = user.FullName,
                Id = user.Id,

            };
     
        }
    }
}
