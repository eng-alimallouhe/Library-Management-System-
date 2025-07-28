using LMS.Application.Abstractions.LibraryManagement;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.DTOs.LibraryManagement.Authors;
using LMS.Application.DTOs.LibraryManagement.Categories;
using LMS.Application.DTOs.LibraryManagement.Genres;
using LMS.Application.DTOs.LibraryManagement.Publishers;
using LMS.Domain.Identity.Enums;
using LMS.Domain.LibraryManagement.Models.Authors;
using LMS.Domain.LibraryManagement.Models.Categories;
using LMS.Domain.LibraryManagement.Models.Genres;
using LMS.Domain.LibraryManagement.Models.Publishers;
using LMS.Infrastructure.Specifications.LibraryManagement.Authors;
using LMS.Infrastructure.Specifications.LibraryManagement.Categories;
using LMS.Infrastructure.Specifications.LibraryManagement.Genres;
using LMS.Infrastructure.Specifications.LibraryManagement.Publishers;

namespace LMS.Infrastructure.Helpers.LibraryManagement
{
    public class StockManagementHelper : IStockManagementHelper
    {
        private readonly ISoftDeletableRepository<Category> _catRepo;
        private readonly ISoftDeletableRepository<Publisher> _publisherRepo;
        private readonly ISoftDeletableRepository<Genre> _genreRepo;
        private readonly ISoftDeletableRepository<Author> _authorRepo;

        public StockManagementHelper(
            ISoftDeletableRepository<Category> catRepo,
            ISoftDeletableRepository<Genre> genreRepo,
            ISoftDeletableRepository<Author> authorRepo,
            ISoftDeletableRepository<Publisher> publisherRepo)
        {
            _catRepo = catRepo;
            _publisherRepo = publisherRepo;
            _genreRepo = genreRepo;
            _authorRepo = authorRepo;
        }

        public async Task<ICollection<CategoryLookUpDto>> GetAllCategoriesAsync(Language language)
        {
            var (items, count) = await _catRepo.GetAllAsync(new CategoryLookupSpecification());

            return items.Select(c => new CategoryLookUpDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.Translations.FirstOrDefault(l => l.Language == language)?.CategoryName ?? "N/A"
            }).ToList();
        }

        public async Task<ICollection<PublisherLookupDto>> GetAllPublishersAsync(Language language)
        { 
            var (items, count) = await _publisherRepo.GetAllAsync(new PublisherLookupSpecification());

            return items.Select(c => new PublisherLookupDto
            {
                PublisherId = c.PublisherId,
                PublisherName = c.Translations.FirstOrDefault(l => l.Language == language)?.PublisherName ?? "N/A"
            }).ToList();
        }

        public async Task<ICollection<GenreLookupDto>> GetAllGenresAsync(Language language)
        {
            var (items, count) = await _genreRepo.GetAllAsync(new GenreLookupSpecification());

            return items.Select(c => new GenreLookupDto
            {
                GenreId = c.GenreId,
                GenreName = c.Translations.FirstOrDefault(l => l.Language == language)?.GenreName ?? "N/A"
            }).ToList();
        }


        public async Task<ICollection<AuthorLookupDto>> GetAllAuthorsAsync(Language language)
        {
            var (items, count) = await _authorRepo.GetAllAsync(new AuthorLookupSpecification());

            return items.Select(c => new AuthorLookupDto
            {
                AuthorId = c.AuthorId,
                AuthorName = c.Translations.FirstOrDefault(l => l.Language == language)?.AuthorName ?? "N/A"
            }).ToList();
        }
    }
}
