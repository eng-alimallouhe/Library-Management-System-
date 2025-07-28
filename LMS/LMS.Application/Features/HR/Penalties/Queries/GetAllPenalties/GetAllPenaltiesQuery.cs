using System.ComponentModel.DataAnnotations;
using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.HR;
using LMS.Application.Filters.HR.EmployeeIdentity;
using MediatR;

namespace LMS.Application.Features.HR.Penalties.Queries.GetAllPenalties
{
    public record GetAllPenaltiesQuery(PenaltyFilter Filter) : IRequest<PagedResult<PenaltyOverviewDto>>;
}
