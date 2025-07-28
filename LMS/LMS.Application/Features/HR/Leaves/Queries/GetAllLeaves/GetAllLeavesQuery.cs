using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.HR;
using LMS.Application.Filters.HR.EmployeeIdentity;
using MediatR;

namespace LMS.Application.Features.HR.Leaves.Queries.GetAllLeaves
{
    public record GetAllLeavesQuery(LeaveFilter Filter) : IRequest<PagedResult<LeaveOverviewDto>>;
}
