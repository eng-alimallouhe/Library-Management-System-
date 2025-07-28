using System.Linq.Expressions;
using LMS.Application.Filters.Inventory;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.Identity.Enums;
using LMS.Domain.LibraryManagement.Models.Products;

namespace LMS.Infrastructure.Specifications.Finacial
{
    public class FilteredDeadStockSpecification : ISpecification<Product>
    {
        private readonly DeadStockFilter _filter;


        public Expression<Func<Product, bool>>? Criteria =>
        p =>
            (_filter.Name == null || p.Translations
            .Any(t => t.Language == (Language) _filter.Language && t.ProductName.Contains(_filter.Name, StringComparison.OrdinalIgnoreCase))) &&
            (!p.OrderItems.Any() ||
            !p.OrderItems
            .Any(oi => oi.Order.CreatedAt >= _filter.From && oi.Order.CreatedAt <= _filter.To));



        public List<string> Includes => ["Translations", "OrderItems.Order"];

        public bool IsTrackingEnabled => false;

        public Expression<Func<Product, object>>? OrderBy =>
            _filter.IsDesc ? null : GetOrderExpression(_filter.OrderBy);

        public Expression<Func<Product, object>>? OrderByDescending =>
            _filter.IsDesc ? GetOrderExpression(_filter.OrderBy) : null;

        public int? Skip => _filter.PageNumber;

        public int? Take => _filter.PageSize;


        private Expression<Func<Product, object>>? GetOrderExpression(DeadStockOrdering? order)
        {
            return order switch
            {
                DeadStockOrdering.ProductStock => r => r.ProductStock,
                DeadStockOrdering.ProductPrice => r => r.ProductPrice,
                DeadStockOrdering.CreatedAt => r => r.CreatedAt,
                DeadStockOrdering.LastMovementDate => r =>
                    r.OrderItems.Any()?  
                    r.OrderItems.Max(oi => oi.CreatedAt) : 
                    r.CreatedAt,
                _ => r => r.CreatedAt
            };
        }


        public FilteredDeadStockSpecification(
            DeadStockFilter filter)
        {
            _filter = filter;
        }
    }
}
