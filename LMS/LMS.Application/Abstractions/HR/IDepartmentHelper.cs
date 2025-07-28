using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.HR.Departments;
using LMS.Application.Filters.HR;
using LMS.Domain.HR.Models;

namespace LMS.Application.Abstractions.HR
{
    public interface IDepartmentHelper
    {
        Task<(ICollection<DepartmentOverviewDto> items, int count)> GetDepartmentsAsync(DepartmentFilter filter);

        Task<ICollection<Department>> GetAvaliableDepartmentsAsync(Guid? employeeId);

        Task<DepartmentDetailsDTO> BuildDepartmentResponseAsync(Department department);
    }
}
