using LMS.Common.Results;
using MediatR;

namespace LMS.Application.Features.Customers.Commands.DeleteCustomer
{
    public class DeleteCustomerCommand : IRequest<Result>
    {
        public Guid CustomerId { get; set; }
    }
}
