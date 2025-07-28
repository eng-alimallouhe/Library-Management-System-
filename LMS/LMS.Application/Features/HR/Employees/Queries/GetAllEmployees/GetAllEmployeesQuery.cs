using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.HR.Employees;
using LMS.Application.Filters.HR;
using MediatR;

namespace LMS.Application.Features.HR.Employees.Queries.GetAllEmployees
{
    public record GetAllEmployeesQuery(
        EmployeeFilter Filter) : IRequest<PagedResult<EmployeeOverviewDto>>;
}