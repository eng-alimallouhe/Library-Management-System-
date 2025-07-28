using AutoMapper;
using LMS.Application.Abstractions;
using LMS.Application.DTOs.HR;
using LMS.Application.Features.HR.Incentives.Commands.CreateIncentive;
using LMS.Application.Features.HR.Incentives.Commands.UpdateIncentive;
using LMS.Domain.HR.Models;

namespace LMS.Application.MappingProfiles.HR
{
    public class IncentiveMappingProfile : Profile
    {
        public IncentiveMappingProfile()
        {
            CreateMap<CreateIncentiveCommand, Incentive>();

            CreateMap<UpdateIncentiveCommand, Incentive>()
                .ForMember(dest => dest.IncentiveId, opt => opt.Ignore());

            CreateMap<Incentive, IncentiveOverviewDto>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FullName))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.DecisionDate.ConvertToSyrianTime()));

            CreateMap<Incentive, IncentivesLookupDto>();

            CreateMap<Incentive, IncentiveDetailsDto>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FullName));
        }
    }
}
