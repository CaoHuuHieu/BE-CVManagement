using CVManagement.Models.Entities;

namespace CVManagement.Repositories.Interfaces
{
    public interface ICVRepository
    {
        public Task<IEnumerable<CV>> GetAll();
        public Task<CV> GetById(long id);
        public Task<bool> Update(CV cv);
        public Task<bool> Save(CV cv);
        public Task<bool> Delete(CV cv);
    }
}
