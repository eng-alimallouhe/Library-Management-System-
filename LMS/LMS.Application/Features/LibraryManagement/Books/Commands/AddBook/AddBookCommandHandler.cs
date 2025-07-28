using LMS.Application.Abstractions.Communications;
using LMS.Application.Abstractions.LibraryManagement;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.Abstractions.UnitOfWorks;
using LMS.Common.Enums;
using LMS.Common.Results;
using LMS.Domain.LibraryManagement.Models.Products;
using LMS.Domain.LibraryManagement.Models.Relations;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Books.Commands.AddBook
{
    public class AddBookCommandHandler : IRequestHandler<AddBookCommand, Result>
    {
        private readonly ISoftDeletableRepository<Book> _bookRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseRepository<GenreBook> _gbRepo;
        private readonly IBaseRepository<ProductTranslation> _btRepo;
        private readonly IBaseRepository<PublisherBook> _pbRepo;
        private readonly IFileHostingUploaderHelper _fileHostingUploaderHelper;



        public AddBookCommandHandler(
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
            _btRepo = btRepo;
        }

        public async Task<Result> Handle(AddBookCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var book = new Book()
                {
                    ISBN = request.ISBN,
                    Pages = request.Pages,
                    ProductPrice = request.ProductPrice,
                    ProductStock = request.ProductStock,
                };

                var uploadResult = await _fileHostingUploaderHelper.UploadImageAsync(request.ImageByte, Guid.NewGuid().ToString() + ".png");

                if (uploadResult.IsFailed)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return Result.Failure($"COMMON.{ResponseStatus.UPLOAD_FAILED}");
                }

                book.ImgUrl = uploadResult.Value!;

                var newGenres = request.Genres.Select(bg => new GenreBook
                {
                    GenreId = bg,
                    BookId = book.ProductId
                }).ToList();

                book.AuthorId = request.AuthorId;

                await _bookRepo.AddAsync(book);

                await _unitOfWork.SaveChangesAsync();

                var transaltions = new List<ProductTranslation>()
                {
                    new ProductTranslation
                    {
                        ProductId = book.ProductId,
                        Language = Domain.Identity.Enums.Language.AR,
                        ProductName = request.ARProductName,
                        ProductDescription = request.ARProductDescription,
                    },
                    new ProductTranslation
                    {
                        ProductId = book.ProductId,
                        Language = Domain.Identity.Enums.Language.EN,
                        ProductName = request.ENProductName,
                        ProductDescription = request.ENProductDescription,
                    }
                };

                var newPublishers = request.Publishers.Select(pb => new PublisherBook
                {
                    PublisherId = pb,
                    BookId = book.ProductId,
                }).ToList();


                await _pbRepo.AddRangeAsync(newPublishers);

                await _gbRepo.AddRangeAsync(newGenres);

                await _btRepo.AddRangeAsync(transaltions);

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