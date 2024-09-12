using AutoMapper;
using OrderSystem.DTOs;
using OrderSystem.Models;

namespace OrderSystem.Mappers
{
    public class GetClientMapper : Profile
    {
        public GetClientMapper()
        {
            CreateMap<Client, GetDtO>()
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ClientName))
                    .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ClientId))
                    .ForMember(dest => dest.FixedPart, opt => opt.MapFrom(src => src.FixedPart));
        }
    }
    public class AddClientMapper : Profile
    {
        public AddClientMapper()
        {
            CreateMap<AddEditDTO, Client>()
                    .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Name))
                    .ForMember(dest => dest.FixedPart, opt => opt.MapFrom(src => src.FixedPart))
                    .ForMember(dest => dest.OrgId, opt => opt.MapFrom(src => src.OrgID));



        }
    }
    
}
