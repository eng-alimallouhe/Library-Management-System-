using MediatR;
using LMS.Common.Results;

namespace LMS.Application.Features.HR.Leaves.Commands.DeleteLeave
{
    public class DeleteLeaveCommand : IRequest<Result>
    {
        public Guid LeaveId { get; set; }
        public Guid RequestingUserId { get; set; }
    }
}