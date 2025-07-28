using System.Linq.Expressions;
using LMS.Application.DTOs.Admin.Dashboard;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.Orders.Models;

namespace LMS.Infrastructure.Specifications.Sales
{
    public class TopSellingProductsSpecification : IProjectedSpecification<OrderItem, TopSellingProductDto, Guid>
    {
        private readonly int _language;


        public Expression<Func<OrderItem, bool>>? Criteria => null;

        public List<string> Includes => new()
        {
            "Product"
        };

        public bool IsTrackingEnabled => false;

        public Expression<Func<OrderItem, object>>? OrderBy => null;

        public Expression<Func<OrderItem, object>>? OrderByDescending => null;

        public int? Skip { get; } = 1;

        public int? Take { get; }


        // ← Group by ProductId
        public Expression<Func<OrderItem, Guid>> GroupBy => oi => oi.ProductId;


        // ← Select projected result
        public Expression<Func<IGrouping<Guid, OrderItem>, TopSellingProductDto>> Selector =>
            group => new TopSellingProductDto
            {
                ProductId = group.Key,
                ProductName = group
                .SelectMany(x => x.Product.Translations
                        .Where(t => (int) t.Language == _language))
                    .Select(t => t.ProductName)
                    .FirstOrDefault() ?? "N/A",
                TotalSold = group.Sum(x => x.Quantity * x.UnitPrice)
            };


        public Func<IQueryable<TopSellingProductDto>, IOrderedQueryable<TopSellingProductDto>>? ResultOrdering =>
            query => query.OrderByDescending(x => x.TotalSold);

        public TopSellingProductsSpecification(int topCount, int language)
        {
            _language = language;
            Take = topCount;
        }
    }
}