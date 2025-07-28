using System.Linq.Expressions;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.LibraryManagement.Models.Products;

namespace LMS.Infrastructure.Specifications.LibraryManagement
{
    public class LowStockQuantitySpecification : ISpecification<Product>
    {
        private readonly int _maxQuantity;

        public Expression<Func<Product, bool>>? Criteria =>
            product => product.ProductStock <= _maxQuantity;

        public List<string> Includes => ["Translations"];

        public bool IsTrackingEnabled => false;

        public Expression<Func<Product, object>>? OrderBy => p => p.ProductStock;

        public Expression<Func<Product, object>>? OrderByDescending => null;

        public int? Skip => null;

        public int? Take => null;


        public LowStockQuantitySpecification(
            int maxQuantity)
        {
            _maxQuantity = maxQuantity;
        }
    }
}
