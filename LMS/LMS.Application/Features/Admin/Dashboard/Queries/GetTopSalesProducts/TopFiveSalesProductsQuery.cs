using LMS.Application.DTOs.Admin.Dashboard;
using MediatR;

namespace LMS.Application.Features.Admin.Dashboard.Queries.GetTopFiveSalesProducts
{
    public record TopFiveSalesProductsQuery(
        int Language) : IRequest<ICollection<TopSellingProductDto>>;
}
