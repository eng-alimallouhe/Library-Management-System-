using LMS.Application.DTOs.HR.Employees;
using MediatR;

namespace LMS.Application.Features.HR.Employees.Queries.SearchEmployeesByName
{
    public record SearchEmployeesByNameQuery(
        string EmployeeName) : IRequest<ICollection<EmployeeLookupDto>>;
}
