using AutoMapper;
using OrderSystem.DTOs;
using OrderSystem.Models;

namespace OrderSystem.Mappers
{
    public class UserMapper
    {
        public class RegisterMapper : Profile
        {
            public RegisterMapper()
            {
                CreateMap<RegisterDTO, User>()
                        .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                        .ForMember(dest => dest.UserFirstName, opt => opt.MapFrom(src => src.FirstName))
                        .ForMember(dest => dest.UserLastName, opt => opt.MapFrom(src => src.LastName))
                        .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

            }
        }
    }
}
