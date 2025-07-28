using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.HR;
using LMS.Application.Filters.HR.EmployeeIdentity;
using MediatR;

namespace LMS.Application.Features.HR.Incentives.Queries.GetAllIncentives
{
    public record GetAllIncentivesQuery(
        IncentiveFilter Filter) : IRequest<PagedResult<IncentiveOverviewDto>>;
}