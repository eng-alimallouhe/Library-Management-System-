using AutoMapper;
using LMS.Application.Abstractions;
using LMS.Application.DTOs.HR;
using LMS.Application.Features.HR.Penalties.Commands.AddPenalty;
using LMS.Application.Features.HR.Penalties.Commands.UpdatePenalty;
using LMS.Domain.HR.Models;

namespace LMS.Application.MappingProfiles.HR
{
    public class PenaltyMappingProfile : Profile
    {
        public PenaltyMappingProfile()
        {
            CreateMap<AddPenaltyCommand, Penalty>()
                .ForMember(dest => dest.PenaltyId, opt => opt.Ignore());

            CreateMap<UpdatePenaltyCommand, Penalty>()
                .ForMember(dest => dest.PenaltyId, opt => opt.Ignore())
                .ForMember(dest => dest.DecisionFileUrl, opt => opt.Ignore());


            CreateMap<Penalty, PenaltyLookupDto>();



            CreateMap<Penalty, PenaltyOverviewDto>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FullName));



            CreateMap<Penalty, PenaltyDetailsDto>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FullName));
        }
    }
}
