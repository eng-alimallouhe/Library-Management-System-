using AutoMapper;
using LMS.Application.Abstractions;
using LMS.Application.Abstractions.LibraryManagement;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.Abstractions.UnitOfWorks;
using LMS.Application.DTOs.LibraryManagement.Authors;
using LMS.Application.DTOs.LibraryManagement.Books;
using LMS.Application.DTOs.LibraryManagement.Categories;
using LMS.Application.DTOs.LibraryManagement.Genres;
using LMS.Application.DTOs.LibraryManagement.Products;
using LMS.Application.DTOs.LibraryManagement.Publishers;
using LMS.Application.DTOs.Stock;
using LMS.Application.Filters.Inventory;
using LMS.Application.Filters.LibraryManagement;
using LMS.Domain.Identity.Enums;
using LMS.Domain.LibraryManagement.Models;
using LMS.Domain.LibraryManagement.Models.Products;
using LMS.Infrastructure.Specifications.LibraryManagement;

namespace LMS.Infrastructure.Helpers.LibraryManagement
{
    public class ProductHelper : IProductHelper
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISoftDeletableRepository<Product> _productRepo;
        private readonly IBaseRepository<InventoryLog> _logRepo;
        private readonly IBaseRepository<ProductTranslation> _ptRepo;
        private readonly IMapper _mapper;
        private readonly ISoftDeletableRepository<Book> _bookRepo;

        public ProductHelper(
            IUnitOfWork unitOfWork,
            ISoftDeletableRepository<Product> productRepo,
            IBaseRepository<InventoryLog> logRepo,
            IBaseRepository<ProductTranslation> ptRepo,
            IMapper mapper,
            ISoftDeletableRepository<Book> bookRepo
            )
        {
            _unitOfWork = unitOfWork;
            _productRepo = productRepo;
            _logRepo = logRepo;
            _ptRepo = ptRepo;
            _mapper = mapper;
            _bookRepo = bookRepo;
        }

        public async Task<(ICollection<Book> items, int count)> GetAllBooksAsync(BooksFilter filter)
        {
            var spec = new FilteredBooksSpecification(filter);

            return await _bookRepo.GetAllAsync(spec);
        }

        public async Task<(ICollection<Product> items, int count)> GetAllProducts(ProductFilter filter)
        {
            var spec = new FilteredProductsSpecification(filter);

            return await _productRepo.GetAllAsync(spec);
        }

        public async Task<BooksDetailsDto?> GetBookAsync(Guid id, Language language)
        {
            var book = await _bookRepo.GetBySpecificationAsync(new BookDetailsSpecification(id));

            if (book == null)
            {
                return null;
            }

            decimal discount = 0;

            var dis = book.Discounts.FirstOrDefault(d => d.IsActive && d.StartDate >= DateTime.UtcNow && d.EndDate <= DateTime.UtcNow);

            if (dis != null)
            {
                discount = dis.DiscountPercentage;
            }

            return new BooksDetailsDto
            {
                BookId = book.ProductId,
                BookName = book.Translations.FirstOrDefault(t => t.Language == language)?.ProductName ?? "N/A",
                BookDescription = book.Translations.FirstOrDefault(t => t.Language == language)?.ProductDescription ?? "N/A",
                ImgUrl = book.ImgUrl,
                ISBN = book.ISBN,
                ProductPrice = book.ProductPrice,
                ProductStock = book.ProductStock,
                Pages = book.Pages,
                PublishedYear = book.PublishedYear,
                Categories = book.ProductCategoriys.Select(b  => new CategoryLookUpDto
                {
                    CategoryId = b.CategoryId,
                    CategoryName = b.Category.Translations.FirstOrDefault(c => c.Language == language)?.CategoryName ?? "N/A"
                }).ToList(),
                Genres = book.GenreBooks.Select(b => new GenreLookupDto
                {
                    GenreId = b.GenreId,
                    GenreName = b.Genre.Translations.FirstOrDefault(g => g.Language == language)?.GenreName ?? "N/A",
                }).ToList(),
                Publishers = book.PublisherBooks.Select(b => new PublisherLookupDto
                {
                    PublisherId = b.PublisherId,
                    PublisherName = b.Publisher.Translations.FirstOrDefault(p => p.Language == language)?.PublisherName ?? "N/A"
                }).ToList(),
                Logs = book.Logs.Select(l => new InventoryLogOverviewDto
                {
                    ProductId = l.ProductId,
                    ProductName = book.Translations.FirstOrDefault(t => t.Language == language)?.ProductName ?? "N/A",
                    LogDate = l.LogDate.ConvertToSyrianTime(),
                    ChangedQuantity = l.ChangedQuantity,
                    ChangeType = l.ChangeType,
                }).ToList(),
                Author = new AuthorLookupDto
                {
                    AuthorId = book.AuthorId,
                    AuthorName = book.Author.Translations.FirstOrDefault(a => a.Language == language)?.AuthorName ?? "N/A"
                },
                DiscountPercentage = discount
            };
        }


        public async Task<Book?> GetBookToUpdateAsync(Guid id)
        {
            return await _bookRepo.GetBySpecificationAsync(new BookDetailsSpecification(id));
        }

        public async Task<BookToUpdate?> GetBookToUpdateAsync(Guid id, Language language)
        {
            var book = await _bookRepo.GetBySpecificationAsync(new BookDetailsSpecification(id));

            if (book == null)
            {
                return null;
            }

            return new BookToUpdate
            {
                ARProductName = book.Translations.FirstOrDefault(t => t.Language == Language.AR)?.ProductName ?? "N/A",
                ARProductDescription = book.Translations.FirstOrDefault(t => t.Language == Language.AR)?.ProductDescription ?? "N/A",
                ENProductName = book.Translations.FirstOrDefault(t => t.Language == Language.EN)?.ProductName ?? "N/A",
                ENProductDescription = book.Translations.FirstOrDefault(t => t.Language == Language.EN)?.ProductDescription ?? "N/A",
                ImgUrl = book.ImgUrl,
                ISBN = book.ISBN,
                ProductPrice = book.ProductPrice,
                ProductStock = book.ProductStock,
                Pages = book.Pages,
                PublishedYear = book.PublishedYear,
                Categories = book.ProductCategoriys.Select(b => new CategoryLookUpDto
                {
                    CategoryId = b.CategoryId,
                    CategoryName = b.Category.Translations.FirstOrDefault(c => c.Language == language)?.CategoryName ?? "N/A"
                }).ToList(),
                Genres = book.GenreBooks.Select(b => new GenreLookupDto
                {
                    GenreId = b.GenreId,
                    GenreName = b.Genre.Translations.FirstOrDefault(g => g.Language == language)?.GenreName ?? "N/A",
                }).ToList(),
                Publishers = book.PublisherBooks.Select(b => new PublisherLookupDto
                {
                    PublisherId = b.PublisherId,
                    PublisherName = b.Publisher.Translations.FirstOrDefault(p => p.Language == language)?.PublisherName ?? "N/A"
                }).ToList(),
                Author = new AuthorLookupDto
                {
                    AuthorId = book.AuthorId,
                    AuthorName = book.Author.Translations.FirstOrDefault(a => a.Language == language)?.AuthorName ?? "N/A"
                }
            };
        }


        public async Task<Product?> GetProductAsync(Guid id)
        {
            return await _productRepo.GetBySpecificationAsync(new ProductDetailsSpecification(id));
        }

        public async Task<ProductDetailsDto?> GetProdyctByIdAsync(Guid id, int lang)
        {
            var product = await _productRepo.GetBySpecificationAsync(new ProductDetailsSpecification(id));

            if (product == null)
            {
                return null;
            }

            var result = _mapper.Map<ProductDetailsDto>(product,
                context =>
                {
                    context.Items["lang"] = lang;
                });

            result.Categories = _mapper.Map<ICollection<CategoryLookUpDto>>(product.ProductCategoriys,
                context =>
                {
                    context.Items["lang"] = lang;
                });

            return result;
        }
    }
}
