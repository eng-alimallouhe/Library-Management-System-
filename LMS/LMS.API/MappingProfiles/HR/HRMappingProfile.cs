using AutoMapper;
using LMS.API.DTOs.HR;
using LMS.Application.Features.HR.Holidays.Commands.CreateHoliday;
using LMS.Application.Features.HR.Holidays.Commands.UpdateHoliday;
using LMS.Application.Features.HR.Incentives.Commands.CreateIncentive;
using LMS.Application.Features.HR.Incentives.Commands.UpdateIncentive;
using LMS.Application.Features.HR.Leaves.Commands.AddLeave;
using LMS.Application.Features.HR.Leaves.Commands.UpdateLeave;
using LMS.Application.Features.HR.Penalties.Commands.AddPenalty;
using LMS.Application.Features.HR.Penalties.Commands.UpdatePenalty;

namespace LMS.API.MappingProfiles.HR
{
    public class HRMappingProfile : Profile
    {
        public HRMappingProfile()
        {
            CreateMap<PenaltyCreateRequestDto, AddPenaltyCommand>()
                .ForMember(dest => dest.DecisionFile, opt => opt.Ignore());

            CreateMap<PenaltyUpdateRequestDto, UpdatePenaltyCommand>()
                .ForMember(src => src.PenaltyId, opt => opt.Ignore());

            CreateMap<IncentiveCreateRequestDto, CreateIncentiveCommand>()
                .ForMember(dest => dest.DecisionFile, opt => opt.Ignore());


            CreateMap<IncentiveUpdateRequestDto, UpdateIncentiveCommand>();

            //Holidays:
            CreateMap<HolidayCreateRequestDto, CreateHolidayCommand>();
            
            CreateMap<HolidayUpdateRequestDto, UpdateHolidayCommand>();
            
            
            
            
            CreateMap<LeaveCreateRequestDto, AddLeaveCommand>();

            CreateMap<LeaveUpdateRequestDto, UpdateLeaveCommand>()
                .ForMember(src => src.LeaveId, opt => opt.Ignore());


        }
    }
}
