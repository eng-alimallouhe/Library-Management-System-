using LMS.Common.Results;
using LMS.Domain.HR.Enums;
using MediatR;

namespace LMS.Application.Features.HR.Leaves.Commands.UpdateLeaveStatus
{
    public class UpdateLeaveStatusCommand : IRequest<Result>
    {
        public Guid LeaveId { get; set; }
        public int NewStatus { get; set; }
        public Guid UpdatedBy { get; set; } 
    }
}
