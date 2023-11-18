using CVManagement.Models.DataTransferObject;
using CVManagement.Models.Entities;

namespace CVManagement.Services.Interfaces
{
    public interface IUserCVService
    {
        public Task<IEnumerable<CurriculumVitaeForCustomer>> GetCVByUserId(long id);
        public Task<CV> GetInfoCVById(long cvId, long userId);
        public Task<UserCV> GetById(long cvId, long userId);
        public Task<bool> Update(long cvId, long userId);
        public Task<bool> Save(UserCV cv);
        public Task<bool> CheckPermission(long cvId, long userId);
        public bool Delete(UserCV cv);
    }
}
