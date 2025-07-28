using System.Linq.Expressions;
using LMS.Application.Filters.HR.EmployeeIdentity;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.HR.Models;

namespace LMS.Infrastructure.Specifications.HR.EmployeeIdentity
{
    public class FilteredPenaltySpecification : ISpecification<Penalty>
    {
        private readonly PenaltyFilter _filter;

        public Expression<Func<Penalty, bool>>? Criteria =>
        p =>
            (_filter.DepartmentIds == null || !_filter.DepartmentIds.Any() ||
            p.Employee.EmployeeDepartments
            .Any(ed => ed.IsActive && _filter.DepartmentIds.Contains(ed.DepartmentId))) &&

            (_filter.Name == null ||
                p.Employee.FullName.ToLower().Contains(_filter.Name.ToLower())) &&

            (_filter.ByIsApproved == null || p.IsApproved == _filter.ByIsApproved) &&
            (_filter.ByIsApplied == null || p.IsApplied == _filter.ByIsApplied) &&

            (_filter.From == null || p.DecisionDate >= _filter.From) &&
            (_filter.To == null || p.DecisionDate <= _filter.To);


        public List<string> Includes => ["Employee.EmployeeDepartments"];

        public bool IsTrackingEnabled => false;

        public Expression<Func<Penalty, object>>? OrderBy => (!_filter.IsDesc)? GetOrderingExpression(_filter.OrderBy) : null;

        public Expression<Func<Penalty, object>>? OrderByDescending => _filter.IsDesc ? GetOrderingExpression(_filter.OrderBy) : null;

        public int? Skip => _filter.PageNumber;

        public int? Take => _filter.PageSize;


        private Expression<Func<Penalty, object>> GetOrderingExpression(PenaltyOrderBy? order)
        {
            return order switch
            {
                PenaltyOrderBy.ByEmployeeName => p => p.Employee.FullName,
                PenaltyOrderBy.ByDate => p => p.DecisionDate,
                PenaltyOrderBy.ByReason => p => p.Reason,
                PenaltyOrderBy.ByIsApproved => p => p.IsApproved,
                PenaltyOrderBy.ByIsApplied => p => p.IsApplied,
                _ => p => p.DecisionDate
            };
        }


        public FilteredPenaltySpecification(
            PenaltyFilter filter)
        {
            _filter = filter;
        }
    }
}
