using LMS.Application.Abstractions.Communications;
using LMS.Application.Abstractions.LibraryManagement;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.Abstractions.UnitOfWorks;
using LMS.Common.Enums;
using LMS.Common.Results;
using LMS.Domain.LibraryManagement.Models.Products;
using LMS.Domain.LibraryManagement.Models.Relations;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Books.Commands.UpdateBook
{
    public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, Result>
    {
        private readonly ISoftDeletableRepository<Book> _bookRepo;
        private readonly IProductHelper _productHelper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseRepository<GenreBook> _gbRepo;
        private readonly IBaseRepository<ProductTranslation> _btRepo;
        private readonly IBaseRepository<PublisherBook> _pbRepo;
        private readonly IFileHostingUploaderHelper _fileHostingUploaderHelper;



        public UpdateBookCommandHandler(
            ISoftDeletableRepository<Book> bookRepo,
            IProductHelper productHelper,
            IUnitOfWork unitOfWork,
            IBaseRepository<GenreBook> gbRepo,
            IBaseRepository<PublisherBook> pbRepo,
            IBaseRepository<ProductTranslation> btRepo,
            IFileHostingUploaderHelper fileHostingUploaderHelper)
        {
            _bookRepo = bookRepo;
            _unitOfWork = unitOfWork;
            _gbRepo = gbRepo;
            _pbRepo = pbRepo;
            _fileHostingUploaderHelper = fileHostingUploaderHelper;
            _productHelper = productHelper;
            _btRepo = btRepo;
        }


        public async Task<Result> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var book = await _bookRepo.GetByIdAsync(request.BookId);

                if (book == null)
                {
                    return Result.Failure($"COMMON.{ResponseStatus.UNABLE_DELETE_NOT_FOUNDED_ELEMENT}");
                }

                var oldTranslations = await _btRepo.GetAllByExpressionAsync(
                    bt => bt.ProductId == request.BookId);

                var oldGenres = await _gbRepo.GetAllByExpressionAsync(
                    gb => gb.BookId == request.BookId);

                var oldPublishers = await _pbRepo.GetAllByExpressionAsync(
                    pb => pb.BookId == request.BookId);

                foreach (var item in oldGenres)
                {
                    await _gbRepo.HardDeleteAsync(item.GenreBookId);
                }

                foreach (var item in oldPublishers)
                {
                    await _pbRepo.HardDeleteAsync(item.PublisherBookId);
                }

                book.ISBN = request.ISBN;
                book.ProductPrice = request.ProductPrice;
                book.ProductStock = request.ProductStock;
                book.Pages = request.Pages;
                book.PublishedYear = request.PublishedYear;

                if (request.ImageByte != null && request.ImageByte.Length > 0)
                {
                    var uploadResult = await _fileHostingUploaderHelper.UploadImageAsync(request.ImageByte, Guid.NewGuid().ToString() + ".png");

                    if (uploadResult.IsFailed)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return Result.Failure($"COMMON.{ResponseStatus.UPLOAD_FAILED}");
                    }

                    book.ImgUrl = uploadResult.Value!;
                }

                var newGenres = request.Genres.Select(bg => new GenreBook
                {
                    GenreId = bg,
                    BookId = request.BookId
                }).ToList();

                var newPublishers = request.Publishers.Select(pb => new PublisherBook
                {
                    PublisherId = pb,
                    BookId = request.BookId
                }).ToList();


                foreach (var item in oldTranslations)
                {
                    if (item.Language == Domain.Identity.Enums.Language.AR)
                    {
                        item.ProductName = request.ARProductName;
                        item.ProductDescription = request.ARProductDescription;
                    }
                    else if (item.Language == Domain.Identity.Enums.Language.EN)
                    {
                        item.ProductName = request.ENProductName;
                        item.ProductDescription = request.ENProductDescription;
                    }
                }


                await _pbRepo.AddRangeAsync(newPublishers);


                await _gbRepo.AddRangeAsync(newGenres);


                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                return Result.Success($"COMMON.{ResponseStatus.UPDATE_COMPLETED}");
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result.Failure($"COMMON.{ResponseStatus.UNKNOWN_ERROR}");
            }
        }
    }
}
