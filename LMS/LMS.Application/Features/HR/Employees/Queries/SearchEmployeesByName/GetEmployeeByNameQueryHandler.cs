using AutoMapper;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.DTOs.HR.Employees;
using LMS.Domain.HR.Models;
using MediatR;

namespace LMS.Application.Features.HR.Employees.Queries.SearchEmployeesByName
{
    public class SearchEmployeesByNameQueryHandler : IRequestHandler<SearchEmployeesByNameQuery, ICollection<EmployeeLookupDto>>
    {
        private readonly ISoftDeletableRepository<Employee> _employeeRepo;
        private readonly IMapper _mapper;


        public SearchEmployeesByNameQueryHandler(
            ISoftDeletableRepository<Employee> employeeRepo,
            IMapper mapper)
        {
            _employeeRepo = employeeRepo;
            _mapper = mapper;
        }

        public async Task<ICollection<EmployeeLookupDto>> Handle(SearchEmployeesByNameQuery request, CancellationToken cancellationToken)
        {
            var reponse = await _employeeRepo.GetAllByExpressionAsync(
                e => e.FullName.Contains(request.EmployeeName, StringComparison.OrdinalIgnoreCase));

            return _mapper.Map<ICollection<EmployeeLookupDto>>(reponse);
        }
    }
}
