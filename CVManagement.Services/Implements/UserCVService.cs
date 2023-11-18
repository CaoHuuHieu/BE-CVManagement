using CVManagement.Models.DataTransferObject;
using CVManagement.Models.Entities;
using CVManagement.Repositories.Interfaces;
using CVManagement.Services.Interfaces;
using System.Security.Claims;

namespace CVManagement.Services.Implements
{
    public class UserCVService : IUserCVService
    {
        private readonly IUserCVRepository _userCVRepository;
        public UserCVService(IUserCVRepository userCVRepository)
        {
            _userCVRepository = userCVRepository;
        }

        public async Task<bool> CheckPermission(long cvId, long userId)
        {
            var userCV = await _userCVRepository.GetById(cvId, userId);
            if (userCV != null)
            {
                userCV.LastView = DateTime.Now;
                userCV.Views++;
                return await _userCVRepository.Update(userCV);
            }
            return false;
        }

        public bool Delete(UserCV cv)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<CurriculumVitaeForCustomer>> GetCVByUserId(long id)
        {
            return await _userCVRepository.GetCVByUserId(id);
        }

        public async Task<UserCV> GetById(long cvId, long userId)
        {
            return await _userCVRepository.GetById(cvId, userId);
        }

        public async Task<bool> Save(UserCV cv)
        {
            return await _userCVRepository.Save(cv);
        }

        public async Task<bool> Update(long svId, long userId)
        {
            throw new NotImplementedException();
        }

        public async Task<CV> GetInfoCVById(long cvId, long userId)
        {
            return await _userCVRepository.GetInfoCVById(cvId, userId);
        }

  
    }
}
