using AutoMapper;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.DTOs.HR.Employees;
using LMS.Domain.HR.Models;
using MediatR;

namespace LMS.Application.Features.HR.Employees.Queries.GetEmployeeForUpdate
{
    public class GetEmployeeForUpdateQueryHandler : IRequestHandler<GetEmployeeForUpdateQuery, EmployeeUpdateDto>
    {
        private readonly ISoftDeletableRepository<Employee> _employeeRepo;
        private readonly IMapper _mapper;


        public GetEmployeeForUpdateQueryHandler(
            ISoftDeletableRepository<Employee> employeeRepo,
            IMapper mapper)
        {
            _employeeRepo = employeeRepo;
            _mapper = mapper;   
        }


        public async Task<EmployeeUpdateDto> Handle(GetEmployeeForUpdateQuery request, CancellationToken cancellationToken)
        {
            var reponse = await _employeeRepo.GetByExpressionAsync(
                e => e.UserId == request.EmployeeId, false);

            var result = _mapper.Map<EmployeeUpdateDto>(reponse);
            
            return result;
        }
    }
}
