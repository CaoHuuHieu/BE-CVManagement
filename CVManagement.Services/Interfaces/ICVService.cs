using CVManagement.Models.DataTransferObject;
using CVManagement.Models.Entities;
using Microsoft.AspNetCore.Http;

namespace CVManagement.Services.Interfaces
{
    public interface ICVService
    {
        public Task<IEnumerable<CurriculumVitaeBasicInfor>> GetAllCurriculumVitaes();
        public Task<CurriculumVitaeBasicInfor> GetCurriculumVitaeById(long cvId);
        public Task<IEnumerable<CurriculumVitaeBasicInfor>> GetAllCurriculumVitaes(long hrId);
        public Task<IEnumerable<CurriculumVitaeBasicInfor>> GetCurriculumVitaesByCustomerId(long customerId);
        public Task<IEnumerable<CurriculumVitaeForCustomer>> GetCurriculumVitaeDetail(long customerId);
        public Task<IEnumerable<CurriculumVitaeForCustomer>> GetCurriculumVitaeDetail(long hrId, long customerId);
        public Task<CurriculumnVitaeFile> ViewById(long cvId);
        public Task<CV> Rename(long id, string name);
        public Task<bool> Save(long posterId, IFormFile[]le);
        public Task<CV> Delete(long id);
        public Task<bool> DeleteCurriculumVitaeOfCustomer(long cvId, long customerId);
    }
}
