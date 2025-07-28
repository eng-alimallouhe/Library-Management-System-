using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.HR.Departments;
using LMS.Application.Filters.HR;
using MediatR;

namespace LMS.Application.Features.HR.Departments.Queries.GetAllDepartments
{
    public record GetAllDepartmentsQuery(
        DepartmentFilter Filter) : IRequest<PagedResult<DepartmentOverviewDto>>;
}
