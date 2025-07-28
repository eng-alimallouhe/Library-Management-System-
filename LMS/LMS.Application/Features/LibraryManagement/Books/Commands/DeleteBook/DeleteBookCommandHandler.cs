using LMS.Application.Abstractions.Repositories;
using LMS.Application.Abstractions.UnitOfWorks;
using LMS.Common.Enums;
using LMS.Common.Results;
using LMS.Domain.LibraryManagement.Models.Products;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Books.Commands.DeleteBook
{
    public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand, Result>
    {
        private readonly ISoftDeletableRepository<Book> _bookRepo;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteBookCommandHandler(
            ISoftDeletableRepository<Book> bookRepo,
            IUnitOfWork unitOfWork)
        {
            _bookRepo = bookRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var book = await _bookRepo.GetByIdAsync(request.BookId);

                if (book == null || book.IsActive == false)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return Result.Success($"COMMON.{ResponseStatus.UNABLE_DELETE_NOT_FOUNDED_ELEMENT}");
                }

                book.IsActive = false;

                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();
                
                return Result.Success($"COMMON.{ResponseStatus.DELETE_COMPLETED}");
            }
            catch
            {

                await _unitOfWork.RollbackTransactionAsync();
                return Result.Success($"COMMON.{ResponseStatus.UNKNOWN_ERROR}");
            }
        }
    }
}
