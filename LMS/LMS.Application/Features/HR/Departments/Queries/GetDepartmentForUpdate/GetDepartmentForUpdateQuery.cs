using LMS.Application.DTOs.HR.Departments;
using MediatR;

namespace LMS.Application.Features.HR.Departments.Queries.GetDepartmentForUpdate
{
    public record GetDepartmentForUpdateQuery(Guid Id): IRequest<DepartmentUpdateDto?>;
}
