using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.Customers.Models;

namespace LMS.Infrastructure.Specifications.Customers
{
    public class ActiveCustomersSpecification : ISpecification<Customer>
    {
        public Expression<Func<Customer, bool>>? Criteria { get; }

        public List<string> Includes => [];

        public bool IsTrackingEnabled => false;

        public Expression<Func<Customer, object>>? OrderBy =>
            c => c.CreatedAt;

        public Expression<Func<Customer, object>>? OrderByDescending => null;

        public int? Skip => null;

        public int? Take => null;


        public ActiveCustomersSpecification(
            DateTime? from, 
            bool isActive)
        {
            Criteria = c =>
                (from == null || c.CreatedAt >= from) &&
                (!isActive || !c.IsDeleted);
        }
    }

}
