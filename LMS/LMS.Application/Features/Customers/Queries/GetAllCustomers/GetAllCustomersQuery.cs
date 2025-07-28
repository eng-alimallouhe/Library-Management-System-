using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.Customers;
using LMS.Application.Filters.Customers;
using MediatR;

namespace LMS.Application.Features.Customers.Queries.GetAllCustomers
{
    public record GetAllCustomersQuery(
        CustomersFilter Filter) : IRequest<PagedResult<CustomersOverViewDto>>;
}
