using LMS.Application.DTOs.HR.Departments;
using MediatR;

namespace LMS.Application.Features.HR.Departments.Queries.GetAvaliableDepartments
{
    public record GetAvaliableDepartmentsQuery(Guid? EmployeeId) 
        : IRequest<ICollection<DepartmentLookupDto>>;
}
