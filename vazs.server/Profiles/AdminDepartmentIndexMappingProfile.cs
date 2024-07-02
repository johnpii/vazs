using AutoMapper;
using Firebase.Database;
using vazs.server.Models;

namespace vazs.server.Profiles
{
    public class AdminDepartmentIndexMappingProfile : Profile
    {
        public AdminDepartmentIndexMappingProfile()
        {
            CreateMap<FirebaseObject<DepartmentModelForDatabase>, DepartmentModelForIndex>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Key))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Object.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Object.Description))
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Object.Image));
        }
    }
}

