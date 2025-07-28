using LMS.Application.DTOs.Customers;
using MediatR;

namespace LMS.Application.Features.Customers.Queries.GetCustomerById
{
    public record GetCustomerByIdQuery(
        Guid userId,
        int Language) : IRequest<CustomerDetailsDto?>;
}
