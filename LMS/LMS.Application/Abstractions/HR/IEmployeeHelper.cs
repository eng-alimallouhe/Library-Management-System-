using LMS.Application.DTOs.HR.Employees;
using LMS.Application.Filters.HR;
using LMS.Common.Results;
using LMS.Domain.HR.Models;

namespace LMS.Application.Abstractions.HR
{
    public interface IEmployeeHelper
    {
        Task<Result<EmployeeCreatedDto>> CreateEmployee(Employee employee, Guid departmenId, byte[] appointmentUrl, byte[] faceImage);

        Task<EmployeeDetailsDto?> BuildEmployeeDetailsAsync(Guid employeeId);

        Task<Result> TransferEmployeeAsync(Guid employeeId, Guid departmentId, byte[] appointment);

        Task<(ICollection<Employee> items, int count)> GetEmployeesPageAsync(EmployeeFilter filter);

        Task<ICollection<Guid>> GetManagersIdsAsync();
    }
}
