using LMS.Application.DTOs.LibraryManagement.Authors;
using LMS.Application.DTOs.LibraryManagement.Categories;
using LMS.Application.DTOs.LibraryManagement.Genres;
using LMS.Application.DTOs.LibraryManagement.Publishers;
using LMS.Domain.Identity.Enums;

namespace LMS.Application.Abstractions.LibraryManagement
{
    public interface IStockManagementHelper
    {
        Task<ICollection<CategoryLookUpDto>> GetAllCategoriesAsync(Language language);

        Task<ICollection<PublisherLookupDto>> GetAllPublishersAsync(Language language);
        Task<ICollection<GenreLookupDto>> GetAllGenresAsync(Language language);
        Task<ICollection<AuthorLookupDto>> GetAllAuthorsAsync(Language language);
    }
}
