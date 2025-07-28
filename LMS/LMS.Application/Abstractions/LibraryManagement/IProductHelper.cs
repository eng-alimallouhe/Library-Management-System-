using LMS.Application.DTOs.LibraryManagement.Books;
using LMS.Application.DTOs.LibraryManagement.Products;
using LMS.Application.Filters.Inventory;
using LMS.Application.Filters.LibraryManagement;
using LMS.Domain.Identity.Enums;
using LMS.Domain.LibraryManagement.Models.Products;

namespace LMS.Application.Abstractions.LibraryManagement
{
    public interface IProductHelper
    {
        Task<(ICollection<Product> items, int count)> GetAllProducts(ProductFilter filter);

        Task<ProductDetailsDto?> GetProdyctByIdAsync(Guid id, int lang);

        Task<Product?> GetProductAsync(Guid id);

        Task<BooksDetailsDto?> GetBookAsync(Guid id, Language language);

        Task<(ICollection<Book> items, int count)> GetAllBooksAsync(BooksFilter filter);
        Task<BookToUpdate?> GetBookToUpdateAsync(Guid id, Language language);

        Task<Book?> GetBookToUpdateAsync(Guid id);
    }
}
