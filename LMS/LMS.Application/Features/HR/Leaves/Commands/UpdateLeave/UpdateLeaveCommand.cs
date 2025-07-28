using LMS.Common.Results;
using LMS.Domain.HR.Enums;
using MediatR;

namespace LMS.Application.Features.HR.Leaves.Commands.UpdateLeave
{
    public class UpdateLeaveCommand : IRequest<Result>
    {
        public Guid LeaveId { get; set; } = Guid.NewGuid();
        public Guid EmployeeId { get; set; }
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; }
        public LeaveType LeaveType { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
