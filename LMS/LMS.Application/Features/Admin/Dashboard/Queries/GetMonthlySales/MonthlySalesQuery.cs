using LMS.Application.DTOs.Admin.Dashboard;
using MediatR;

namespace LMS.Application.Features.Admin.Dashboard.Queries.GetMonthlySales
{
    public record MonthlySalesQuery(
        DateTime From,
        DateTime To) : IRequest<ICollection<MonthlySalesDto>>;
}
