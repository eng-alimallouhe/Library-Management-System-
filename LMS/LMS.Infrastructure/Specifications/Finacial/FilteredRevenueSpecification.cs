using System.Linq.Expressions;
using LMS.Application.Filters.Finacial;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.Financial.Models;

namespace LMS.Infrastructure.Specifications.Finacial
{
    public class FilteredRevenueSpecification : ISpecification<Revenue>
    {
        private readonly RevenueFilter _filter;

        public FilteredRevenueSpecification(RevenueFilter filter)
        {
            _filter = filter;
        }

        public Expression<Func<Revenue, bool>>? Criteria =>
            r =>
                (_filter.CustomerIds == null || _filter.CustomerIds.Any() || 
                _filter.CustomerIds.Contains(r.CustomerId)) &&
                
                (_filter.EmployeeIds == null || _filter.EmployeeIds.Any() ||
                _filter.EmployeeIds.Contains(r.EmployeeId)) &&
                
                (_filter.Service == null || r.Service == _filter.Service) &&
                
                (_filter.MinAmount == null || r.Amount >= _filter.MinAmount) &&
                
                (_filter.MaxAmount == null || r.Amount <= _filter.MaxAmount) &&
                
                (_filter.From == null || r.CreatedAt >= _filter.From) &&
                    
                (_filter.To == null || r.CreatedAt <= _filter.To);

        public List<string> Includes => ["Customer", "Employee"];

        public bool IsTrackingEnabled => false;

        public Expression<Func<Revenue, object>>? OrderBy =>
            !_filter.Desc ? GetOrderExpression(_filter.OrderBy) : null;

        public Expression<Func<Revenue, object>>? OrderByDescending =>
            _filter.Desc ? GetOrderExpression(_filter.OrderBy) : null;

        public int? Skip => _filter.PageNumber;
        public int? Take => _filter.PageSize;

        private Expression<Func<Revenue, object>>? GetOrderExpression(RevenueOrdering? order)
        {
            return order switch
            {
                RevenueOrdering.Service => r => r.Service,
                RevenueOrdering.Amount => r => r.Amount,
                RevenueOrdering.Date => r => r.CreatedAt,
                _ => r => r.CreatedAt
            };
        }
    }
}
