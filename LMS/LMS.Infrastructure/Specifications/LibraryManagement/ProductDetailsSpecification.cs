using System.Linq.Expressions;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.LibraryManagement.Models.Products;

namespace LMS.Infrastructure.Specifications.LibraryManagement
{
    public class ProductDetailsSpecification : ISpecification<Product>
    {
        private readonly Guid _id;

        public Expression<Func<Product, bool>>? Criteria => 
            p => p.ProductId == _id;

        public List<string> Includes => new List<string>
        {
            nameof(Product.Translations),
            "ProductCategoriys.Category.Translations",
            nameof(Product.Logs),
        };

        public bool IsTrackingEnabled => false;

        public Expression<Func<Product, object>>? OrderBy => null;

        public Expression<Func<Product, object>>? OrderByDescending => null;

        public int? Skip => null;

        public int? Take => null;

        public ProductDetailsSpecification(Guid id)
        {
            _id = id;
        }
    }
}
