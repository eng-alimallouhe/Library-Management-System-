using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.Financial;
using LMS.Application.Filters.Finacial;
using MediatR;

namespace LMS.Application.Features.Accounting.Revenues.Queries.GetRevenues
{
    public record GetRevenuesQuery(
        RevenueFilter Filter): IRequest<PagedResult<RevenueDetailsDto>>;
}
