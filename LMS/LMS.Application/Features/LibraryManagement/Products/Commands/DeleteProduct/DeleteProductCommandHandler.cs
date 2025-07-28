using LMS.Application.Abstractions.Repositories;
using LMS.Application.Abstractions.UnitOfWorks;
using LMS.Common.Enums;
using LMS.Common.Results;
using LMS.Domain.LibraryManagement.Models.Products;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Products.Commands.DeleteProduct
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result>
    {
        private readonly ISoftDeletableRepository<Product> productRepo;
        private readonly IUnitOfWork unitOfWork;

        public DeleteProductCommandHandler(
            ISoftDeletableRepository<Product> productRepo,
            IUnitOfWork unitOfWork)
        {
            this.productRepo = productRepo;
            this.unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var product = await productRepo.GetByIdAsync(request.ProductId);

                if (product == null || product.IsActive == false)
                {
                    return Result.Failure($"COMMON.{ResponseStatus.UNABLE_DELETE_NOT_FOUNDED_ELEMENT}");
                }

                product.IsActive = false;

                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success($"COMMON.{ResponseStatus.DELETE_COMPLETED}");
            }
            catch
            {
                return Result.Failure($"COMMON.{ResponseStatus.UNABLE_DELETE_ELEMENT}");
            }
        }
    }
}
