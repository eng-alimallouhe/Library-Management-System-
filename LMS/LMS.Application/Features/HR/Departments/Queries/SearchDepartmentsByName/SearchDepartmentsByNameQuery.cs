using LMS.Application.DTOs.HR.Departments;
using MediatR;

namespace LMS.Application.Features.HR.Departments.Queries.SearchDepartmentsByName
{
    public record SearchDepartmentsByNameQuery(
        string? DepartmentName) : IRequest<ICollection<DepartmentLookupDto>>;
}
