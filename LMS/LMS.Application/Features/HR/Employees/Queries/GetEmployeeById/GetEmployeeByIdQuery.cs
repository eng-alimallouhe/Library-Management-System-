using LMS.Application.DTOs.HR.Employees;
using MediatR;

namespace LMS.Application.Features.HR.Employees.Queries.GetEmployeeById
{
    public record GetEmployeeByIdQuery(
        Guid Id) : IRequest<EmployeeDetailsDto?>;
}