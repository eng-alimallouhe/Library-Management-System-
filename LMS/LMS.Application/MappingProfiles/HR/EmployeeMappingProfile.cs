using AutoMapper;
using LMS.Application.DTOs.HR.Employees;
using LMS.Application.Features.HR.Employees.Command.CreateEmployee;
using LMS.Application.Features.HR.Employees.Command.TransferEmployee;
using LMS.Application.Features.HR.Employees.Command.UpdateEmployee;
using LMS.Application.MappingProfiles.HR.MappingSettings;
using LMS.Domain.HR.Models;

namespace LMS.Application.MappingProfiles.HR
{
    public class EmployeeMappingProfile : Profile
    {
        public EmployeeMappingProfile()
        {
            CreateMap<Employee, EmployeeOverviewDto>()
                .ConvertUsing<EmployeeToOverviewConverter>();

            CreateMap<Employee, EmployeeDetailsDto>()
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.BaseSalary, opt => opt.MapFrom(src => src.BaseSalary))
                .ForMember(dest => dest.CurrentDepartmentName, opt => opt.MapFrom(src =>
                    src.EmployeeDepartments.FirstOrDefault(ed => ed.IsActive).Department.DepartmentName ?? "N/A"))
                .ForMember(dest => dest.DepartmentsHistory, opt => opt.MapFrom(src =>
                    src.EmployeeDepartments.OrderByDescending(ed => ed.StartDate)))
                .ForMember(dest => dest.Leaves, opt => opt.MapFrom(src =>
                    src.Leaves.Where(l => l.IsActive))) 
                .ForMember(dest => dest.Salaries, opt => opt.MapFrom(src =>
                    src.Salaries.Where(s => s.IsActive))) 
                .ForMember(dest => dest.Penalties, opt => opt.MapFrom(src =>
                    src.Penalties.Where(p => p.IsActive)))
                .ForMember(dest => dest.Incentives, opt => opt.MapFrom(src =>
                    src.Incentives.Where(i => i.IsActive)))
                .ForMember(dest => dest.Attendances, opt => opt.MapFrom(src =>
                    src.Attendances.Where(a => a.IsActive && a.Date.Month == DateTime.Now.Month).OrderBy(a => a.Date)));



            CreateMap<CreateEmployeeCommand, Employee>();

            CreateMap<UpdateEmployeeCommand, Employee>();

            CreateMap<Employee, EmployeeUpdateDto>();

            CreateMap<TransferEmployeeCommand, EmployeeDepartment>()
                .ForMember(dest => dest.AppointmentDecisionUrl, opt => opt.MapFrom(src => src.AppointmentDecision));
        }
    }
}
