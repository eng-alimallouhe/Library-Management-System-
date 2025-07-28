using LMS.Common.Results;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Products.Commands.DeleteProduct
{
    public class DeleteProductCommand : IRequest<Result>
    {
        public Guid ProductId { get; set; }
    }
}
