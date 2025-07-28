using LMS.Application.Abstractions.HR;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.DTOs.HR.Departments;
using LMS.Domain.HR.Models;
using MediatR;

namespace LMS.Application.Features.HR.Departments.Queries.GetDepartmentById
{
    public class GetDepartmentByIdQueryHandler : IRequestHandler<GetDepartmentByIdQuery, DepartmentDetailsDTO?>
    {
        private readonly ISoftDeletableRepository<Department> _departmentRepo;
        private readonly IDepartmentHelper _departmentHelper;

        public GetDepartmentByIdQueryHandler(
            ISoftDeletableRepository<Department> departmentRepo,
            IDepartmentHelper departmentHelper)
        {
            _departmentRepo = departmentRepo;
            _departmentHelper = departmentHelper;
        }

        public async Task<DepartmentDetailsDTO?> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
        {
            var department = await _departmentRepo.GetByIdAsync(request.Id);

            if (department is null)
            {
                return null;
            }

            return await _departmentHelper.BuildDepartmentResponseAsync(department);
        }
    }
}
