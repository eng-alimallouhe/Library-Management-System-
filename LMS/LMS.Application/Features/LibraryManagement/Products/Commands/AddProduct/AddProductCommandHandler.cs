using LMS.Application.Abstractions.Communications;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.Abstractions.UnitOfWorks;
using LMS.Common.Enums;
using LMS.Common.Results;
using LMS.Domain.LibraryManagement.Models;
using LMS.Domain.LibraryManagement.Models.Products;
using LMS.Domain.LibraryManagement.Models.Relations;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Products.Commands.AddProduct
{
    public class AddProductCommandHandler : IRequestHandler<AddProductCommand, Result>
    {
        private readonly ISoftDeletableRepository<Product> _productRepo;
        private readonly IBaseRepository<ProductTranslation> _ptRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileHostingUploaderHelper _fileHostingUploaderHelper;
        private readonly IBaseRepository<InventoryLog> _logRepo;
        private readonly IBaseRepository<ProductCategory> _pcRepo;

        public AddProductCommandHandler(
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

        public async Task<Result> Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();


            try
            {
                var uploadResult = await _fileHostingUploaderHelper.UploadImageAsync(request.ImageFile, Guid.NewGuid().ToString());

                if (uploadResult.IsFailed)
                {
                    return Result.Failure($"COMMON.{ResponseStatus.UPLOAD_FAILED}");
                }

                var product = new Product()
                {
                    ProductPrice = request.ProductPrice,
                    ProductStock = request.ProductStock,
                    ImgUrl = uploadResult.Value!
                };

                var log = new InventoryLog()
                {
                    ProductId = product.ProductId,
                    ChangedQuantity = request.ProductStock,
                    ChangeType = Domain.LibraryManagement.Enums.LogType.Adding,
                    LogDate = DateTime.UtcNow
                };

                var translations = new List<ProductTranslation>
                {
                    new ProductTranslation()
                    {
                        ProductId = product.ProductId,
                        Language = Domain.Identity.Enums.Language.AR,
                        ProductName = request.ARProductName,
                        ProductDescription = request.ARProductDescription,
                    },
                    new ProductTranslation()
                    {
                        ProductId = product.ProductId,
                        Language = Domain.Identity.Enums.Language.EN,
                        ProductName = request.ENProductName,
                        ProductDescription = request.ENProductDescription,
                    }
                };

                var cats = new List<ProductCategory>();


                foreach (var item in request.CategoriesIds)
                {
                    cats.Add(new ProductCategory
                    {
                        ProductId = product.ProductId,
                        CategoryId = item
                    });
                }

                await _productRepo.AddAsync(product);
                
                await _unitOfWork.SaveChangesAsync();

                await _ptRepo.AddRangeAsync(translations);
                
                await _logRepo.AddAsync(log);

                await _pcRepo.AddRangeAsync(cats);

                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();
                
                return Result.Success($"COMMON.{ResponseStatus.ADD_COMPLETED}");
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                
                return Result.Failure($"COMMON.{ResponseStatus.UNKNOWN_ERROR}");
            }
        }
    }
}
