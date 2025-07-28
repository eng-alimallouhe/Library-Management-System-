using System.Linq.Expressions;
using LMS.Application.Filters.Finacial;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.Financial.Models;

namespace LMS.Infrastructure.Specifications.Finacial
{
    public class FilteredPaymentsSpecification : ISpecification<Payment>
    {
        private readonly PaymentFilter _filter;

        public FilteredPaymentsSpecification(
            PaymentFilter filter)
        {
            _filter = filter;
        }

        public Expression<Func<Payment, bool>>? Criteria => 
            p =>
                (_filter.EmployeeIds == null || _filter.EmployeeIds.Any() ||  
                _filter.EmployeeIds.Contains(p.EmployeeId)) &&
                (_filter.MinAmount == null || p.Amount >= _filter.MinAmount) &&
                (_filter.MaxAmount == null || p.Amount <= _filter.MaxAmount) &&
                (_filter.From == null || p.Date >= _filter.From) &&
                (_filter.To == null || p.Date <= _filter.To);


        public List<string> Includes => ["Employee"];

        public bool IsTrackingEnabled => false;

        public Expression<Func<Payment, object>>? OrderBy =>
            !_filter.IsDesc ? GetOrderExpression(_filter.Ordering) : null;

        public Expression<Func<Payment, object>>? OrderByDescending =>
            !_filter.IsDesc ? GetOrderExpression(_filter.Ordering) : null;

        public int? Skip => _filter.PageNumber;

        public int? Take => _filter.PageSize;



        private Expression<Func<Payment, object>>? GetOrderExpression(PaymentOrdering? order)
        {
            return order switch
            {
                PaymentOrdering.ByEmployeeName => p => p.Employee.FullName,
                PaymentOrdering.ByAmount => p => p.Amount,
                PaymentOrdering.ByDate => p => p.Date,
                _ => p => p.Date
            };
        }
    }
}
