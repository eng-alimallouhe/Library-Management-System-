using System.Linq.Expressions;
using LMS.Application.Filters.Inventory;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.Identity.Enums;
using LMS.Domain.LibraryManagement.Models.Products;

namespace LMS.Infrastructure.Specifications.LibraryManagement
{
    public class FilteredProductsSpecification : ISpecification<Product>
    {
        private readonly ProductFilter _filter;

        public Expression<Func<Product, bool>>? Criteria =>
        p =>
            (_filter.Name == null ||
                p.Translations.Any(t =>
                    t.ProductName.Contains(_filter.Name))) &&

            (_filter.From == null || p.CreatedAt >= _filter.From) &&
            
            (_filter.To == null || p.CreatedAt <= _filter.To) &&

            (_filter.CategoryId == null || !_filter.CategoryId.Any() ||
            p.ProductCategoriys.Any(pc => _filter.CategoryId.Contains(pc.CategoryId))) &&
        
            (_filter.MinPrice == null || p.ProductPrice >= _filter.MinPrice) &&
        
            (_filter.MaxPrice == null || p.ProductPrice <= _filter.MaxPrice) &&
        
            (!p.ProductCategoriys.Any(c => c.Category.Translations.Any(t => t.CategoryName.ToLower().Contains("book")))) &&

            (_filter.MinQuantity == null || p.ProductStock >= _filter.MinQuantity) &&
            
            (_filter.MaxQuantity == null || p.ProductStock <= _filter.MaxQuantity);

        public List<string> Includes => new()
        {
            "ProductCategoriys.Category.Translations",
            "Translations",
            nameof(Product.Discounts)
        };

        public bool IsTrackingEnabled => false;

        public Expression<Func<Product, object>>? OrderBy => p => p.ProductId;

        public Expression<Func<Product, object>>? OrderByDescending => null;

        public int? Skip => _filter.PageNumber;

        public int? Take => _filter.PageSize;

        public FilteredProductsSpecification(ProductFilter filter)
        {
            _filter = filter;
        }
    }
}
