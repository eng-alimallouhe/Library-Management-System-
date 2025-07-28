using AutoMapper;
using LMS.Application.DTOs.HR;
using LMS.Domain.HR.Models;

namespace LMS.Application.MappingProfiles.HR
{
    public class AttendanceMappingProfile : Profile
    {
        public AttendanceMappingProfile()
        {

            CreateMap<Attendance, AttendanceLookup>()
                .ForMember(des => des.Day, opt => opt.MapFrom(src => (int) src.Date.DayOfWeek));
                

            CreateMap<Attendance, AttendanceOverviewDto>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FullName));


            CreateMap<Salary, SalariesOverviewDto>();
        }
    }
}
