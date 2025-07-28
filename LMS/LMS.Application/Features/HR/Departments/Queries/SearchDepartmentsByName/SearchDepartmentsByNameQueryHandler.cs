using AutoMapper;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.Abstractions.Services.Admin;
using LMS.Application.DTOs.HR.Departments;
using LMS.Domain.HR.Models;
using MediatR;

namespace LMS.Application.Features.HR.Departments.Queries.SearchDepartmentsByName
{
    public class SearchDepartmentsByNameQueryHandler : IRequestHandler<SearchDepartmentsByNameQuery, ICollection<DepartmentLookupDto>>
    {
        private readonly ISoftDeletableRepository<Department> _departmentRepo;
        private readonly IMapper _mapper;

        public SearchDepartmentsByNameQueryHandler(
            ISoftDeletableRepository<Department> departmentRepo,
            IMapper mapper)
        {
            _departmentRepo = departmentRepo;
            _mapper = mapper;
        }


        public async Task<ICollection<DepartmentLookupDto>> Handle(SearchDepartmentsByNameQuery request, CancellationToken cancellationToken)
        {
             ICollection<Department> departments = [];
 
            if (request.DepartmentName is not null)
            {
                departments = await _departmentRepo.GetAllByExpressionAsync(
                    d => d.DepartmentName.ToLower().Contains(request.DepartmentName.ToLower()));
            }
            else
            {
                departments = await _departmentRepo.GetAllByExpressionAsync(
                    d => d.DepartmentName.ToLower().Contains(""));
            }
            return _mapper.Map<ICollection<DepartmentLookupDto>>(departments);
        }
    }
}
