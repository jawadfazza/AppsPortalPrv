
using AppsPortal.Models;
using AppsPortal.ViewModels;
using AutoMapper;

namespace AppsPortal.Library
{
    public class CMSMappingProfiles : Profile
    {
        public CMSMappingProfiles()
        {
            CreateMap<string, string>().ConvertUsing(str => (str ?? "").Trim());

            CreateMap<userRegistrationQueue, userAccounts>();
            CreateMap<userRegistrationQueue, userPersonalDetails>();
            CreateMap<userRegistrationQueue, userPersonalDetailsLanguage>();
            CreateMap<userRegistrationQueue, userServiceHistory>();

            CreateMap<userPersonalDetailsLanguage, userPersonalDetailsLanguage>().ReverseMap();

            //CreateMap<Test, Test>().ForMember(dest => dest.RegistrationDate, opt => opt.MapFrom(src => Portal.LocalTime(src.RegistrationDate)));
            //CreateMap<Test, Test>().ForMember(dest => dest.RegistrationDate, opt => opt.MapFrom(src => Portal.UTCTime(src.RegistrationDate)));

        }
    }
}