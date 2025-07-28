using LMS.Common.Results;
using MediatR;

namespace LMS.Application.Features.HR.Employees.Command.DeleteEmployee
{
    public class DeleteEmployeeCommand : IRequest<Result>
    {
        public Guid EmployeeId { get; set; }
    }
}
