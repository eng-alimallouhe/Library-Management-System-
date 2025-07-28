using System.Linq.Expressions;
using LMS.Application.Filters.Customers;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.Customers.Models;

namespace LMS.Infrastructure.Specifications.Customers
{
    public class InActiveCustomersSpecification : ISpecification<Customer>
    {
        private readonly CustomersFilter _filter;

        public InActiveCustomersSpecification(CustomersFilter filter)
        {
            _filter = filter;
        }

        public Expression<Func<Customer, bool>>? Criteria =>
        c => 
            (_filter.Name == null || c.FullName.ToLower().Contains(_filter.Name)) &&
            !c.Orders.Any()
            || !c.Orders.Any(o => o.CreatedAt >= _filter.From) &&
            (!_filter.OnlyActiveRegisters || !c.IsDeleted)
;
        
        public List<string> Includes => ["Orders"];

        public bool IsTrackingEnabled => false;

        public Expression<Func<Customer, object>>? OrderBy =>
            !_filter.IsDesc ? GetOrderingExpression(_filter.OrderBy) : null;


        public Expression<Func<Customer, object>>? OrderByDescending =>
            _filter.IsDesc ? GetOrderingExpression(_filter.OrderBy) : null;


        public int? Skip => _filter.PageNumber;


        public int? Take => _filter.PageSize;

        private Expression<Func<Customer, object>> GetOrderingExpression(CustomerOrderBy order)
        {
            return order switch
            {
                CustomerOrderBy.ByName => c => c.FullName,
                CustomerOrderBy.ByRegistrationDate => c => c.CreatedAt,
                _ => c => c.FullName
            };
        }
    }
}
