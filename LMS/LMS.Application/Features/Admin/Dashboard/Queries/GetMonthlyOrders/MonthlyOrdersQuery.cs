using LMS.Application.DTOs.Admin.Dashboard;
using MediatR;

namespace LMS.Application.Features.Admin.Dashboard.Queries.GetMonthlyOrders
{
    public record MonthlyOrdersQuery(
        DateTime From,
        DateTime To) : IRequest<ICollection<MonthlyOrdersDto>>;
}