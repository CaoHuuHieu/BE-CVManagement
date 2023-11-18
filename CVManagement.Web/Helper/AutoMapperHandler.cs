using AutoMapper;
using CVManagement.Models.DataTransferObject;
using CVManagement.Models.Entities;

namespace CVManagement.Web.Helper
{
    public class AutoMapperHandler : Profile
    {
        public AutoMapperHandler()
        {
            CreateMap<CV, CurriculumVitaeBasicInfor>()
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FileName))
              .ForMember(dest => dest.Poster, opt => opt.MapFrom(src => new UserBasicInfor
              {
                  Id = src.Poster.Id,
                  FullName = src.Poster.FullName,
                  Avatar = src.Poster.Avatar
              }));

            CreateMap<UserCV, CurriculumVitaeBasicInfor>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CV.FileName))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Views > 0 ? 1:0))
               .ForMember(dest => dest.Poster, opt => opt.MapFrom(src => new UserBasicInfor
               {
                   Id = src.Sender.Id,
                   FullName = src.Sender.FullName,
                   Avatar = src.Sender.Avatar
               }));

            CreateMap<UserCV, CurriculumVitaeForCustomer>()
            .ForMember(dest => dest.CurriculumVitae,
              opt => opt.MapFrom(src => new CurriculumVitaeBasicInfor
              {
                  Id = src.CVId,
                  Name = src.CV.FileName,
                  FileUrl = src.CV.FileUrl,
                  UploadDate = src.CV.UploadDate,
                  Poster = new UserBasicInfor { FullName = src.Sender.FullName, Company = src.Sender.Company }
              }));


            CreateMap<User, UserBasicInfor>();
           
        }
     

    }
}
