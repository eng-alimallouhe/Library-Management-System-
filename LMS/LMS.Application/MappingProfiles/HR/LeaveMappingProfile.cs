using AutoMapper;
using LMS.Application.Abstractions;
using LMS.Application.DTOs.HR;
using LMS.Application.Features.HR.Leaves.Commands.AddLeave;
using LMS.Application.Features.HR.Leaves.Commands.UpdateLeave;
using LMS.Domain.HR.Enums;
using LMS.Domain.HR.Models;

namespace LMS.Application.Mappings.HR
{
    public class LeaveMappingProfile : Profile
    {
        public LeaveMappingProfile()
        {
            CreateMap<Leave, LeaveDetailsDto>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FullName));

            
            CreateMap<Leave, LeaveOverviewDto>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FullName));

            
            CreateMap<AddLeaveCommand, Leave>()
                .ForMember(dest => dest.LeaveType, opt => opt.MapFrom(src => (LeaveType)src.LeaveType));

            
            CreateMap<UpdateLeaveCommand, Leave>()
                .ForMember(dest => dest.LeaveType, opt => opt.MapFrom(src => (LeaveType)src.LeaveType));

            CreateMap<Leave, LeaveUpdateDto>();
        }
    }
}
