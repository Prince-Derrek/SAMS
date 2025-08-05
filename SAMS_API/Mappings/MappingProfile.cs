using AutoMapper;
using SamsApi.DTOs;
using SamsApi.Models;

namespace SamsApi.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDTO>()
            .ForMember(dest => dest.role, opt => opt.MapFrom(src => src.Role.Name))
            .ReverseMap()
            .ForMember(dest => dest.Role, opt => opt.Ignore());

            CreateMap<UserDTO, User>();

            CreateMap<RegisterUserDTO, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserSecret, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.isActive, opt => opt.Ignore())
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName));
        }
    }
}
