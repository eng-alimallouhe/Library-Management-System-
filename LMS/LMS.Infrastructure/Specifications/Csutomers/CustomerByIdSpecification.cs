using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.Customers.Models; // عدل حسب مكان موديل العميل عندك

namespace LMS.Infrastructure.Specifications.Customers
{
    public class CustomerByIdSpecification : ISpecification<Customer>
    {
        private readonly Guid _customerId;

        public CustomerByIdSpecification(Guid customerId)
        {
            _customerId = customerId;
        }

        public Expression<Func<Customer, bool>>? Criteria =>
            c => c.UserId == _customerId;

        public List<string> Includes => new List<string>
        {
            nameof(Customer.Addresses),
            nameof(Customer.Level),
            nameof(Customer.Orders),
            nameof(Customer.Revenues)
        };

        public bool IsTrackingEnabled => false;

        public Expression<Func<Customer, object>>? OrderBy => null;

        public Expression<Func<Customer, object>>? OrderByDescending => null;

        public int? Skip => null;

        public int? Take => null;
    }
}
