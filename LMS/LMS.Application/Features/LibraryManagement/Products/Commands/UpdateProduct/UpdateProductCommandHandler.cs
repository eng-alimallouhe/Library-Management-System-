using LMS.Application.Abstractions.Communications;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.Abstractions.UnitOfWorks;
using LMS.Common.Results;
using LMS.Domain.LibraryManagement.Models.Products;
using LMS.Domain.LibraryManagement.Models.Relations;
using LMS.Domain.LibraryManagement.Models;
using MediatR;
using LMS.Common.Enums;

namespace LMS.Application.Features.LibraryManagement.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result>
    {
        private readonly ISoftDeletableRepository<Product> _productRepo;
        private readonly IBaseRepository<ProductTranslation> _ptRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileHostingUploaderHelper _fileHostingUploaderHelper;
        private readonly IBaseRepository<InventoryLog> _logRepo;
        private readonly IBaseRepository<ProductCategory> _pcRepo;

        public UpdateProductCommandHandler(
            ISoftDeletableRepository<Product> productRepo,
            IBaseRepository<ProductTranslation> ptRepo,
            IUnitOfWork unitOfWork,
            IFileHostingUploaderHelper fileHostingUploaderHelper,
            IBaseRepository<InventoryLog> logRepo,
            IBaseRepository<ProductCategory> pcRepo)
        {
            _productRepo = productRepo;
            _ptRepo = ptRepo;
            _unitOfWork = unitOfWork;
            _fileHostingUploaderHelper = fileHostingUploaderHelper;
            _logRepo = logRepo;
            _pcRepo = pcRepo;
        }

        public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var product = await _productRepo.GetByIdAsync(request.ProductId);

                if (product == null)
                {
                    return Result.Failure($"COMMON.{ResponseStatus.RECORD_NOT_FOUND}");
                }

                var oldTranslations = await _ptRepo.GetAllByExpressionAsync(
                    pt => pt.ProductId == request.ProductId);

                var oldCategories = await _pcRepo.GetAllByExpressionAsync(
                    pc => pc.ProductId == request.ProductId);

                var newTranslations = new List<ProductTranslation>()
                {
                    new ProductTranslation
                    {
                        ProductId = request.ProductId,
                        Language = Domain.Identity.Enums.Language.AR,
                        ProductName = request.ARProductName,
                        ProductDescription = request.ARProductDescription,
                    },
                    new ProductTranslation
                    {
                        ProductId = request.ProductId,
                        Language = Domain.Identity.Enums.Language.EN,
                        ProductName = request.ENProductName,
                        ProductDescription = request.ENProductDescription,
                    }
                };

                var newCats = new List<ProductCategory>();

                foreach (var item in request.CategoriesIds)
                {
                    newCats.Add(new ProductCategory
                    {
                        CategoryId = item,
                        ProductId = request.ProductId,
                    });
                }

                foreach (var item in oldCategories)
                {
                    await _pcRepo.HardDeleteAsync(item.ProductCategoryId);
                }


                foreach (var item in oldTranslations)
                {
                    await _ptRepo.HardDeleteAsync(item.TranslationId);
                }

                product.ProductStock = request.ProductStock;
                product.ProductPrice = request.ProductPrice;

                if (request.ImageFile != null)
                {
                    var uploadeResult = await _fileHostingUploaderHelper.UploadImageAsync(request.ImageFile, Guid.NewGuid().ToString());

                    if (uploadeResult.IsSuccess)
                    {
                        product.ImgUrl = uploadeResult.Value!;
                    }
                }

                await _ptRepo.AddRangeAsync(newTranslations);
                await _pcRepo.AddRangeAsync(newCats);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync();
                return Result.Success($"COMMON.{ResponseStatus.UPDATE_COMPLETED}");
            }
            catch
            {
                return Result.Failure($"COMMON.{ResponseStatus.UNKNOWN_ERROR}");
            }
        }
    }
}
