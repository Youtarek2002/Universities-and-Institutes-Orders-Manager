using AutoMapper;
using OrderSystem.DTOs;
using OrderSystem.Models;

namespace OrderSystem.Mappers
{
    public class GetOrderMapper : Profile
    {
        public GetOrderMapper() { 
        CreateMap<Order,GetOrderDTO>()
                .ForMember(dest => dest.OrderID, opt => opt.MapFrom(src => src.OrderId))
                .ForMember(dest => dest.OrderName, opt => opt.MapFrom(src => src.OrderName))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
                .ForMember(dest => dest.NumberOfCopies, opt => opt.MapFrom(src => src.NumberOfCopies))
                .ForMember(dest => dest.DateIn, opt => opt.MapFrom(src => src.DateIn))
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.Status.StatusName))
                .ForMember(dest => dest.ClientName,opt=>opt.MapFrom(src => src.Client.ClientName))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName)).ReverseMap();


        }
    }

    public class AddEditOrderMapper : Profile
    {
        public AddEditOrderMapper()
        {
            CreateMap<AddEDITOrderDTO, Order>()
                    .ForMember(dest => dest.OrderName, opt => opt.MapFrom(src => src.OrderName))
                    .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
                    .ForMember(dest => dest.NumberOfCopies, opt => opt.MapFrom(src => src.NumberOfCopies));
                    

        }
    }
}
