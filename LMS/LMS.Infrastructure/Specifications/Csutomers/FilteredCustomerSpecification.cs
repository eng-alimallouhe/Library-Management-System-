using System.Linq.Expressions;
using LMS.Application.Filters.Customers;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.Customers.Models;

namespace LMS.Infrastructure.Specifications.Customers
{
    public class FilteredCustomerSpecification : ISpecification<Customer>
    {
        private readonly CustomersFilter _filter;

        public FilteredCustomerSpecification(CustomersFilter filter)
        {
            _filter = filter;
        }

        public Expression<Func<Customer, bool>>? Criteria =>
            c =>
                (_filter.Name == null || c.FullName.ToLower().Contains(_filter.Name.ToLower())) &&
                (!_filter.OnlyActiveRegisters || !c.IsDeleted) &&
                (_filter.From == null || c.CreatedAt >= _filter.From) &&
                (_filter.To == null || c.CreatedAt <= _filter.To);

        public List<string> Includes => new List<string>();
        
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
