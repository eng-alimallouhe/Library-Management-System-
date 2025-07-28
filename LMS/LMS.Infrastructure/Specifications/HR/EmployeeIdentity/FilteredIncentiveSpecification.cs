using System.Linq.Expressions;
using LMS.Application.Filters.HR.EmployeeIdentity;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.HR.Models;

namespace LMS.Infrastructure.Specifications.HR.EmployeeIdentity
{
    public class FilteredIncentiveSpecification : ISpecification<Incentive>
    {
        private readonly IncentiveFilter _filter;

        public Expression<Func<Incentive, bool>>? Criteria =>
            i =>
                (_filter.Name == null || i.Employee.FullName.ToLower().Contains(_filter.Name.ToLower())) &&
                (_filter.DepartmentIds == null || !_filter.DepartmentIds.Any() ||
                    i.Employee.EmployeeDepartments.Any(ed => ed.IsActive && _filter.DepartmentIds.Contains(ed.DepartmentId))) &&
                (_filter.ByIsApproved == null || i.IsApproved == _filter.ByIsApproved) &&
                (_filter.ByIsPaid == null || i.IsPaid == _filter.ByIsPaid) &&
                (_filter.From == null || i.DecisionDate >= _filter.From) &&
                (_filter.To == null || i.DecisionDate <= _filter.To);

        public List<string> Includes => ["Employee.EmployeeDepartments"];

        public bool IsTrackingEnabled => false;

        public Expression<Func<Incentive, object>>? OrderBy => (!_filter.IsDesc) ? GetOrderingExpression(_filter.OrderBy) : null;

        public Expression<Func<Incentive, object>>? OrderByDescending => _filter.IsDesc ? GetOrderingExpression(_filter.OrderBy) : null;

        public int? Skip => _filter.PageNumber;
        public int? Take => _filter.PageSize;

        private Expression<Func<Incentive, object>> GetOrderingExpression(IncentiveOrderBy? order)
        {
            return order switch
            {
                IncentiveOrderBy.ByEmployeeName => i => i.Employee.FullName,
                IncentiveOrderBy.ByDate => i => i.DecisionDate,
                IncentiveOrderBy.ByReason => i => i.Reason,
                IncentiveOrderBy.ByIsApproved => i => i.IsApproved,
                IncentiveOrderBy.ByIsPaid => i => i.IsPaid,
                _ => i => i.DecisionDate
            };
        }

        public FilteredIncentiveSpecification(IncentiveFilter filter)
        {
            _filter = filter;
        }
    }
}