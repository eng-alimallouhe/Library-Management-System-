using AutoMapper;
using LMS.Application.DTOs.HR.Departments;
using LMS.Application.Features.HR.Departments.Command.CreateDepartment;
using LMS.Application.Features.HR.Departments.Command.UpdateDepartment;
using LMS.Application.MappingProfiles.HR.MappingSettings;
using LMS.Domain.HR.Models;

namespace LMS.Application.MappingProfiles.HR
{
    public class DepartmentMappingProfile : Profile
    {
        public DepartmentMappingProfile()
        {
            CreateMap<Department, DepartmentOverviewDto>()
                    .ForMember(dest => dest.EmployeesCount, otp => otp.MapFrom(src => src.EmployeeDepartments.Count(em => em.IsActive)));

            CreateMap<Department, DepartmentDetailsDTO>();
            
            CreateMap<CreateDepartmentCommand, Department>();

            CreateMap<Department, DepartmentLookupDto>();

            CreateMap<EmployeeDepartment, DepartmentHistoryDto>()
                .ForMember(dest => dest.JoinDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.LeaveDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.DepartmentName))
                .ForMember(dest => dest.IsCurrent, opt => opt.MapFrom(src => src.IsActive));

            CreateMap<UpdateDepartmentCommand, Department>();
        }
    }

}
