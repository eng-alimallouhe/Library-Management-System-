using AutoMapper;
using LMS.Application.Abstractions;
using LMS.Application.DTOs.Customers;
using LMS.Application.Features.Authentication.Register.Commands.CreateTempAccount;
using LMS.Application.MappingProfiles.Customers.MappingsSettings;
using LMS.Domain.Customers.Models;

namespace LMS.Application.MappingProfiles.Customers
{
    public class CustomerMappingProfile : Profile
    {
        public CustomerMappingProfile()
        {
            CreateMap<CreateTempAccountCommand, Customer>()
                .ForMember(dest => dest.HashedPassword, opt => opt.MapFrom(src => src.Password));


            CreateMap<Customer, CustomersOverViewDto>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.TotalAmountSpent, opt => opt.MapFrom(src => src.Orders.Sum(fr => fr.Cost)))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ConvertToSyrianTime()))
                .ForMember(dest => dest.LastLogIn, opt => opt.MapFrom(src => src.LastLogIn.ConvertToSyrianTime()));



            CreateMap<Customer, InActiveCustomersDto>()
                .ConvertUsing<CutomerToInactiveCustomerConverter>();


            CreateMap<Customer, CustomerDetailsDto>()
                .ConvertUsing<CustomerToDetailsConverter>();
        }



    }
}