using System.Linq.Expressions;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.HR.Models;

namespace LMS.Infrastructure.Specifications.HR.EmployeeIdentity
{
    public class PenaltyDetailsSpecification : ISpecification<Penalty>
    {
        private readonly Guid _id;

        public Expression<Func<Penalty, bool>>? Criteria =>
            p => p.PenaltyId == _id;

        public List<string> Includes => ["Employee"];

        public bool IsTrackingEnabled => false;

        public Expression<Func<Penalty, object>>? OrderBy => null;

        public Expression<Func<Penalty, object>>? OrderByDescending => null;

        public int? Skip => null;

        public int? Take => null;

        public PenaltyDetailsSpecification(Guid id)
        {
            _id = id;
        }
    }
}
