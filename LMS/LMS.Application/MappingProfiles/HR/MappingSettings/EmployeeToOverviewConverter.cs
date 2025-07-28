using AutoMapper;
using LMS.Application.Abstractions;
using LMS.Application.DTOs.HR.Employees;
using LMS.Domain.HR.Models;

namespace LMS.Application.MappingProfiles.HR.MappingSettings
{
    //Mapping Configurations:
    public class EmployeeToOverviewConverter : ITypeConverter<Employee, EmployeeOverviewDto>
    {
        public EmployeeOverviewDto Convert(Employee source, EmployeeOverviewDto destination, ResolutionContext context)
        {
            return new EmployeeOverviewDto
            {
                EmployeeId = source.UserId,
                FullName = source.FullName,
                HireDate = source.HireDate.ConvertToSyrianTime(),
                CurrentDepartmentName = source.EmployeeDepartments
                    .FirstOrDefault(ed => ed.IsActive)?.Department?.DepartmentName ?? "N/A"
            };
        }
    }
}
