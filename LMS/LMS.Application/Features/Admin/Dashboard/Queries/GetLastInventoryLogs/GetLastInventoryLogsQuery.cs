using LMS.Application.DTOs.Stock;
using LMS.Domain.Identity.Enums;
using MediatR;

namespace LMS.Application.Features.Admin.Dashboard.Queries.GetLastInventoryLogs
{
    public record GetLastInventoryLogsQuery(
        int Language) : IRequest<ICollection<InventoryLogOverviewDto>>;
}
