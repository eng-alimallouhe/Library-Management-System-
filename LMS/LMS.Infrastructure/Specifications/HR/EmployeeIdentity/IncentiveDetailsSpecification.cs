using System.Linq.Expressions;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.HR.Models;

namespace LMS.Infrastructure.Specifications.HR.EmployeeIdentity
{
    public class IncentiveDetailsSpecification : ISpecification<Incentive>
    {
        private readonly Guid _incentiveId;

        public IncentiveDetailsSpecification(Guid incentiveId)
        {
            _incentiveId = incentiveId;
        }

        public Expression<Func<Incentive, bool>>? Criteria => i => i.IncentiveId == _incentiveId;

        public List<string> Includes => ["Employee"];

        public bool IsTrackingEnabled => false;

        public Expression<Func<Incentive, object>>? OrderBy => null;

        public Expression<Func<Incentive, object>>? OrderByDescending => null;

        public int? Skip => null;

        public int? Take => null;
    }
}
