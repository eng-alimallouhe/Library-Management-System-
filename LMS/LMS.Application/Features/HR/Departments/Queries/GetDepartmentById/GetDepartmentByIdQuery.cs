using LMS.Application.DTOs.HR.Departments;
using MediatR;

namespace LMS.Application.Features.HR.Departments.Queries.GetDepartmentById
{
    public record GetDepartmentByIdQuery(Guid Id) : IRequest<DepartmentDetailsDTO?>;
}
