using AutoMapper;
using LMS.Application.Abstractions;
using LMS.Application.DTOs.Financial;
using LMS.Domain.Financial.Models;

namespace LMS.Application.MappingProfiles.Financials
{
    public class FinancialMappingProfile : Profile
    {
        public FinancialMappingProfile()
        {
            CreateMap<Revenue, RevenueOverviewDto>();


            CreateMap<Revenue, RevenueDetailsDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.FullName ?? "N/A"))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FullName ?? "N/A"))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.CreatedAt.ConvertToSyrianTime()));


            CreateMap<Payment, PaymentsDetailsDto>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FullName ?? "N/A"))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.ConvertToSyrianTime()));

            CreateMap<Revenue, RevenueOverviewDto>()
               .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.CreatedAt));
        }
    }
}
