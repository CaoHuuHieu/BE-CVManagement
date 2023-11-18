using CVManagement.Exceptions;
using CVManagement.Models.DataTransferObject;
using CVManagement.Models.Entities;
using CVManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CVManagement.Repositories.Implements
{
    public class UserCVRepository : IUserCVRepository
    {
        private readonly DataContext _context;
        public UserCVRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserCV>> GetAll()
        {
            return await _context.UserCVs.ToListAsync();
        }

        public async Task<UserCV> GetById(long cvId, long userId)
        {
            UserCV userCv = await _context.UserCVs.SingleOrDefaultAsync(u => u.CustomerId == userId && u.CVId == cvId);
            return userCv;
          
        }

        public async Task<ICollection<UserCV>> GetByCustomerId(long customerId)
        {
            return await _context.UserCVs.Include(uc => uc.CV).Include(uc => uc.Sender).Where(uc => uc.CustomerId == customerId).ToListAsync();
        }

        public async Task<ICollection<UserCV>?> GetByUserId(long userId)
        {
           return await _context.UserCVs.Where(uc => uc.CustomerId == userId).ToListAsync();
        }

        public async Task<IEnumerable<CurriculumVitaeForCustomer>> GetCVByUserId(long id)
        {
            var cv1 = _context.UserCVs
                .Join(_context.CVs, userCV => userCV.CVId, cv => cv.Id, (userCV, cv) => new { UserCV = userCV, CV = cv })
                .Where(joinResult => joinResult.UserCV.CustomerId == id)
                   .Select(joinResult => new CurriculumVitaeForCustomer
                   {
                       CurriculumVitae = new CurriculumVitaeBasicInfor
                       {
                           Id = joinResult.CV.Id,
                           Name = joinResult.CV.FileName,
                           UploadDate = joinResult.CV.UploadDate,
                       },
                       Views = joinResult.UserCV.Views,
                       LastView = joinResult.UserCV.LastView
                   });
            return await cv1.ToListAsync();
        }

        public async Task<CV> GetInfoCVById(long cvId, long userId)
        {
            var cv = await _context.UserCVs.Where(uc => uc.CustomerId == userId && uc.CVId == cvId)
                .Include(uc => uc.CV).FirstOrDefaultAsync();
            if (cv != null)
                return cv.CV;
            else
                throw new EntityException("Entity not found!");
        }

        public async Task<bool> Save(UserCV cv)
        {
            if (cv != null)
            {
                await _context.UserCVs.AddAsync(cv);
                return await _context.SaveChangesAsync() > 0;
            }
            else
                throw new EntityException("Entity is not valid!");
        }
        public async Task<bool> Update(UserCV cv)
        {
            UserCV userCv = await _context.UserCVs.FirstOrDefaultAsync(uc => uc.CustomerId == cv.CustomerId && uc.CVId == cv.CVId);
            if (userCv != null)
            {
                _context.UserCVs.Update(userCv);
                return await _context.SaveChangesAsync() > 0;
            }
            else
                throw new EntityException("Entity is not found!");
        }

     public async Task<bool> Delete(UserCV userCv)
        {
            if (userCv != null)
            {
                _context.UserCVs.Remove(userCv);
                return await _context.SaveChangesAsync() > 0;
            }
            else
                throw new EntityException("Entity is not found!");
        }
    }
}
