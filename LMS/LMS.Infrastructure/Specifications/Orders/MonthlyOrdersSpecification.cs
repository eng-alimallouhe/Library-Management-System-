using System.Linq.Expressions;
using LMS.Application.DTOs.Admin.Dashboard;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.Orders.Enums;
using LMS.Domain.Orders.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Specifications.Orders
{
    public class MonthlyOrdersSpecification : IProjectedSpecification<Order, MonthlyOrdersDto, object>
    {
        public Expression<Func<Order, object>> GroupBy =>
                order => new { order.CreatedAt.Year, order.CreatedAt.Month };

        public Expression<Func<IGrouping<object, Order>, MonthlyOrdersDto>> Selector =>
            group => new MonthlyOrdersDto
            {
                Year = EF.Property<int>(group.Key, "Year"),
                Month = EF.Property<int>(group.Key, "Month"),
                TotalOrdersCount = group.Count()
            };

        public Func<IQueryable<MonthlyOrdersDto>, IOrderedQueryable<MonthlyOrdersDto>>? ResultOrdering =>
            query => query.OrderByDescending(x => x.Month);

        public Expression<Func<Order, bool>>? Criteria { get; }

        public List<string> Includes => [];

        public bool IsTrackingEnabled => false;

        public Expression<Func<Order, object>>? OrderBy => null;

        public Expression<Func<Order, object>>? OrderByDescending => null;

        public int? Skip => null;

        public int? Take => null;

        public MonthlyOrdersSpecification(DateTime from, DateTime to)
        {
            Criteria = order => 
                    order.CreatedAt >= from && order.CreatedAt <= to && 
                    order.PaymentStatus == PaymentStatus.Paid;
        }
    }
}
