using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.HR.Models;

namespace LMS.Infrastructure.Specifications.HR.EmployeeIdentity
{
    public class LeaveDetailsSpecification : ISpecification<Leave>
    {
        public Expression<Func<Leave, bool>>? Criteria { get; }
        public List<string> Includes { get; } = new List<string>();
        public bool IsTrackingEnabled => false;
        public Expression<Func<Leave, object>>? OrderBy => null;
        public Expression<Func<Leave, object>>? OrderByDescending => null;
        public int? Skip => null;
        public int? Take => null;

        public LeaveDetailsSpecification(Guid leaveId)
        {
            Criteria = l => l.LeaveId == leaveId;
            Includes.Add(nameof(Leave.Employee));
        }
    }
}