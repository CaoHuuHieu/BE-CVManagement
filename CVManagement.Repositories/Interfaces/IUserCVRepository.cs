using CVManagement.Models.DataTransferObject;
using CVManagement.Models.Entities;

namespace CVManagement.Repositories.Interfaces
{
    public interface IUserCVRepository
    {
        public Task<IEnumerable<UserCV>> GetAll();
        public Task<IEnumerable<CurriculumVitaeForCustomer>> GetCVByUserId(long id);
        public Task<CV> GetInfoCVById(long cvId, long userId);
        public Task<UserCV> GetById(long cvId, long userId);
        public Task<bool> Update(UserCV cv);
        public Task<bool> Save(UserCV cv);
        public Task<bool> Delete(UserCV userCv);
        public Task<ICollection<UserCV>?> GetByUserId(long userId);
        public Task<ICollection<UserCV>> GetByCustomerId( long customerId);

    }
}
