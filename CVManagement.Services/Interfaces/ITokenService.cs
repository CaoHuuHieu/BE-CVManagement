using CVManagement.Models.DataTransferObject;
using CVManagement.Models.Entities;

namespace CVManagement.Services.Interfaces
{
    public interface ITokenService
    {
        public JWTTokenResponse GetToken(User user);
        public string GetReFreshToken();
    }
}
