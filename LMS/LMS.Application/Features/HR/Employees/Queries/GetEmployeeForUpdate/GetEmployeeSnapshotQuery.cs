using LMS.Application.DTOs.HR.Employees;
using MediatR;

namespace LMS.Application.Features.HR.Employees.Queries.GetEmployeeForUpdate
{
    public record GetEmployeeForUpdateQuery(Guid EmployeeId): IRequest<EmployeeUpdateDto>;
}
