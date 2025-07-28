using LMS.Application.DTOs.HR;
using MediatR;

namespace LMS.Application.Features.HR.Leaves.Queries.GetLeaveToUpdate
{
    public record GetLeaveToUpdateQuery(Guid LeaveId) : IRequest<LeaveUpdateDto?>;
}
