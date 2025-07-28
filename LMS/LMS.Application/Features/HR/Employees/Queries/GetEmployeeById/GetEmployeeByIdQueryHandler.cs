using LMS.Application.Abstractions.HR;
using LMS.Application.DTOs.HR.Employees;
using MediatR;

namespace LMS.Application.Features.HR.Employees.Queries.GetEmployeeById
{
    public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, EmployeeDetailsDto?>
    {
        private readonly IEmployeeHelper _employeeHelper;

        public GetEmployeeByIdQueryHandler(
            IEmployeeHelper employeeHelper)
        {
            _employeeHelper = employeeHelper;
        }

        public async Task<EmployeeDetailsDto?> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
        {
            return await _employeeHelper.BuildEmployeeDetailsAsync(request.Id);
        }
    }
}