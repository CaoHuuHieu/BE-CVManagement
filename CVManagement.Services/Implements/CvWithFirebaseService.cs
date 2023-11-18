using AutoMapper;
using CVManagement.Exceptions;
using CVManagement.Models.DataTransferObject;
using CVManagement.Models.Entities;
using CVManagement.Repositories.Interfaces;
using CVManagement.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;

namespace CVManagement.Services.Implements
{
    public class CvWithFirebaseService : ICVService
    {
        private readonly IMapper _mapper;
        private readonly IStorageService _storageService;
        private readonly ICVRepository _cvRepository;
        private readonly IUserCVRepository _userCVRepos;
        public CvWithFirebaseService(IStorageService storageService, ICVRepository cVRepository,   IUserCVRepository userCVRepos, IMapper mapper)
        {
            _storageService = storageService;
            _cvRepository = cVRepository;
            _userCVRepos = userCVRepos;
            _mapper = mapper;
        }
        public async Task<CV> Delete(long id)
        {
            CV cv = await _cvRepository.GetById(id);
            if(cv != null)
            {
                bool check = await _cvRepository.Delete(cv);
                if (check)
                {
                   await _storageService.DeleteFile(cv.UploadName);
                   return cv;
                }
            }
            throw new EntityException("CV is not found to delete!");
        }
     
        public async Task<IEnumerable<CurriculumVitaeBasicInfor>> GetAllCurriculumVitaes()
        {
            var curriculumVitaes = await _cvRepository.GetAll();
            return this._mapper.Map<List<CurriculumVitaeBasicInfor>>(curriculumVitaes);
        }

        public async Task<IEnumerable<CurriculumVitaeBasicInfor>> GetAllCurriculumVitaes(long hrId)
        {
            var curriculumVitaes = await _cvRepository.GetAll();
            return this._mapper.Map<List<CurriculumVitaeBasicInfor>>(curriculumVitaes);
        }
        public async Task<IEnumerable<CurriculumVitaeBasicInfor>> GetCurriculumVitaesByCustomerId(long customerId)
        {
            var userCVs = await _userCVRepos.GetByCustomerId(customerId);
            return _mapper.Map<List<CurriculumVitaeBasicInfor>>(userCVs);
         
        }

        public async Task<IEnumerable<CurriculumVitaeForCustomer>> GetCurriculumVitaeDetail(long customerId)
        {
            var userCVs = await _userCVRepos.GetByCustomerId(customerId);
            var curriculumVitaes = new HashSet<CurriculumVitaeForCustomer>();
            return _mapper.Map<List<CurriculumVitaeForCustomer>>(userCVs);
         
        }
        public async Task<IEnumerable<CurriculumVitaeForCustomer>> GetCurriculumVitaeDetail(long hrId, long customerId)
        {
            var userCVs = await _userCVRepos.GetByCustomerId(customerId);
            var curriculumVitaes = new HashSet<CurriculumVitaeForCustomer>();
            return _mapper.Map<List<CurriculumVitaeForCustomer>>(curriculumVitaes);
        }
        public async Task<CurriculumnVitaeFile> ViewById(long cvId)
        {
            var cv = await _cvRepository.GetById(cvId);
            if (cv != null)
            {
                string contentType = "";
                if (cv.UploadName.EndsWith(".pdf"))
                {
                    contentType = "application/pdf";
                }
                else if (cv.UploadName.EndsWith(".docx"))
                {
                    contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                }
                else if (cv.UploadName.EndsWith(".doc"))
                {
                    contentType = "application/msword";
                }
                var bytes = await _storageService.ViewFileByName(cv.FileUrl);

                return new CurriculumnVitaeFile
                {
                    FileByte = bytes, ContentType = contentType, fileUrl = cv.FileUrl};
            }
            throw new EntityException("CV is not found.");
        }

        public async Task<CV> Rename(long id, string newName)
        {
            CV cv = await _cvRepository.GetById(id);
            if (cv != null)
            {
                var extension = cv.FileName.Substring(cv.FileName.LastIndexOf('.'));
                cv.FileName = newName + extension;
                if (await this._cvRepository.Update(cv))
                    return cv;
            }
            throw new EntityException("CV is not found to rename!");
        }
      
        public async Task<bool> Save(long posterId, IFormFile[] files)
        {
            foreach (var file in files)
            {
                string uploadName = Guid.NewGuid().ToString() +  file.FileName.Substring(file.FileName.LastIndexOf("."));
                string fileUrl = await _storageService.StoreFile(file, uploadName);
                var cv = new CV()
                {
                    FileName = file.FileName,
                    UploadName = uploadName,
                    PosterId = posterId,
                    FileUrl = fileUrl
                };
                await _cvRepository.Save(cv);
            }
            return true;
        }
     
        public async Task<bool> DeleteCurriculumVitaeOfCustomer(long cvId, long customerId)
        {
            var usercv = await _userCVRepos.GetById(cvId, customerId);
            if (usercv != null)
            {
                return await _userCVRepos.Delete(usercv);
            }
            else
                throw new EntityException("UserCV not found.");
        }

        public async Task<CurriculumVitaeBasicInfor> GetCurriculumVitaeById(long cvId)
        {
            var curriculumVitae = await _cvRepository.GetById(cvId);
            return _mapper.Map<CurriculumVitaeBasicInfor>(curriculumVitae);
          
        }
    }
}
