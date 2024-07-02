using AutoMapper;
using Firebase.Database;
using vazs.server.Models;

namespace vazs.server.Profiles
{
    public class TSIndexMappingProfile : Profile
    {
        public TSIndexMappingProfile()
        {
            CreateMap<FirebaseObject<TSModelForDelete>, TSModelForIndex>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Key))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Object.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Object.Description))
            .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => src.Object.CreationDate))
            .ForMember(dest => dest.Deadline, opt => opt.MapFrom(src => src.Object.Deadline))
            .ForMember(dest => dest.Budget, opt => opt.MapFrom(src => src.Object.Budget))
            .ForMember(dest => dest.DocumentExt, opt => opt.MapFrom(src => src.Object.DocumentExt))
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Object.DepartmentName))
            .ForMember(dest => dest.DownloadUrl, opt => opt.MapFrom(src => src.Object.DownloadUrl));
        }
    }
}
