using AutoMapper;
using Firebase.Database;
using vazs.server.Models;

namespace vazs.server.Profiles
{
    public class HomeDepartmentIndexMappingProfile : Profile
    {
        public HomeDepartmentIndexMappingProfile()
        {
            CreateMap<FirebaseObject<DepartmentModelForDatabase>, DepartmentModelForDatabase>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Object.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Object.Description))
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Object.Image));
        }
    }
}
