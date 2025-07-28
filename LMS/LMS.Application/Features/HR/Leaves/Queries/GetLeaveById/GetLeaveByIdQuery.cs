using LMS.Application.DTOs.HR;
using LMS.Common.Results;
using MediatR;

namespace LMS.Application.Features.HR.Leaves.Queries.GetLeaveById
{
    public record GetLeaveByIdQuery(Guid LeaveId) : IRequest<LeaveDetailsDto?>;
}