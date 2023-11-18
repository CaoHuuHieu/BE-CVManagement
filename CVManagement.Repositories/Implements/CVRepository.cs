using CVManagement.Exceptions;
using CVManagement.Models.Entities;
using CVManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CVManagement.Repositories.Implements
{
    public class CVRepository : ICVRepository
    {
        private readonly DataContext _context;
        public CVRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<bool> Delete(CV cv)
        {
            CV cvEntity = await _context.CVs.FindAsync(cv.Id);
            if (cvEntity != null)
            {
                _context.CVs.Remove(cvEntity);
                return await _context.SaveChangesAsync() > 0;
            }
            else
                throw new EntityException("CV is not found!");
        }

        public async Task<IEnumerable<CV>> GetAll()
        {
            return await _context.CVs.Include(cv => cv.Poster).OrderByDescending(cv => cv.UploadDate).ToListAsync();
        }

        public async Task<CV> GetById(long id)
        {
            CV cvEntity = await _context.CVs.Include(cv => cv.Poster).FirstOrDefaultAsync(cv => cv.Id == id);
            if (cvEntity != null)
            {
                return cvEntity;
            }
            else
                throw new EntityException("CV is not found!");
        }

        public async Task<bool> Save(CV cv)
        {
            if (cv != null)
            {
                 _context.CVs.Add(cv);
                return await _context.SaveChangesAsync() > 0;
            }
            else 
                throw new EntityException("CV is not valid!");
        }

        public async Task<bool> Update(CV cv)
        {
            CV cvEntity = await _context.CVs.FindAsync(cv.Id);
            if (cvEntity != null)
            {
                 _context.CVs.Update(cv);
                return await _context.SaveChangesAsync() > 0;
            }
            else
                throw new EntityException("CV is not found!");
        }
    
    }
}
