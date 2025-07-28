using LMS.Common.Results;
using MediatR;

namespace LMS.Application.Features.HR.Employees.Command.TransferEmployee
{
    public class TransferEmployeeCommand : IRequest<Result>
    {
        public Guid EmployeeId { get; set; }
        public Guid DepartmentId { get; set; }
        public byte[] AppointmentDecision { get; set; }
    }
}
