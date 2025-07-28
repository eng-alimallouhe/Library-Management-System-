using LMS.Common.Results;
using MediatR;

namespace LMS.Application.Features.Customers.Commands.ActivateCustomer
{
    public class ActivateCustomerCommand : IRequest<Result>
    {
        public Guid CustomerId { get; set; }
    }
}