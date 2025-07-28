using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.Customers;
using LMS.Application.Filters.Customers;
using MediatR;

namespace LMS.Application.Features.Admin.Customers.Queries.GetInActiveCustomers
{
    public record GetInActiveCustomersQuery(
        CustomersFilter Filter) : IRequest<PagedResult<InActiveCustomersDto>>;
}
