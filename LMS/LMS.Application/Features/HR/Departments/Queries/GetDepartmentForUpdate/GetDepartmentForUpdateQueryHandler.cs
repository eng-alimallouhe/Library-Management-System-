using LMS.Application.Abstractions.Repositories;
using LMS.Application.DTOs.HR.Departments;
using LMS.Domain.HR.Models;
using MediatR;

namespace LMS.Application.Features.HR.Departments.Queries.GetDepartmentForUpdate
{
    public class GetDepartmentForUpdateQueryHandler : IRequestHandler<GetDepartmentForUpdateQuery, DepartmentUpdateDto?>
    {
        private readonly ISoftDeletableRepository<Department> _departmentRepo;

        public GetDepartmentForUpdateQueryHandler(
            ISoftDeletableRepository<Department> departmentRepo)
        {
            _departmentRepo = departmentRepo;
        }

        public async Task<DepartmentUpdateDto?> Handle(GetDepartmentForUpdateQuery request, CancellationToken cancellationToken)
        {
            var dep = await _departmentRepo.GetByIdAsync(request.Id);

            if (dep == null)
            {
                return null;
            }

            var response = new DepartmentUpdateDto
            {
                DepartmentName = dep.DepartmentName,
                DepartmentDescription = dep.DepartmentDescription
            };

            return response;
        }
    }
}
