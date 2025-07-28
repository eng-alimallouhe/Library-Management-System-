using System.Linq.Expressions;
using LMS.Application.DTOs.Admin.Dashboard;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.Orders.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Specifications.Sales
{
    public class MonthlySalesFromBaseOrdersSpecification
        : IProjectedSpecification<Order, MonthlySalesDto, object> // ← تغيير نوع المفتاح لـ object
    {
        public Expression<Func<Order, bool>>? Criteria { get; }

        public List<string> Includes => new();

        public bool IsTrackingEnabled => false;

        public Expression<Func<Order, object>>? OrderBy => null;
        public Expression<Func<Order, object>>? OrderByDescending => null;

        public int? Skip => null;
        public int? Take => null;

        public Expression<Func<Order, object>> GroupBy =>
            order => new { order.CreatedAt.Year, order.CreatedAt.Month };

        public Expression<Func<IGrouping<object, Order>, MonthlySalesDto>> Selector =>
            group => new MonthlySalesDto
            {
                Year = EF.Property<int>(group.Key, "Year"),
                Month = EF.Property<int>(group.Key, "Month"),
                TotalSales = group.Sum(order => order.Cost)
            };

        public Func<IQueryable<MonthlySalesDto>, IOrderedQueryable<MonthlySalesDto>>? ResultOrdering =>
            query => query.OrderByDescending(x => x.TotalSales);

        public MonthlySalesFromBaseOrdersSpecification(
            DateTime from,
            DateTime to)
        {
            Criteria = o => o.CreatedAt >= from && o.CreatedAt <= to;
        }
    }
}
