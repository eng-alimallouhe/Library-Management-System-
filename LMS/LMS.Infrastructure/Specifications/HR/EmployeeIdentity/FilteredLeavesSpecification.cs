using LMS.Application.Filters.HR.EmployeeIdentity;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.HR.Models;
using System.Linq.Expressions;

namespace LMS.Infrastructure.Specifications.HR.EmployeeIdentity
{
    public class FilteredLeaveSpecification : ISpecification<Leave>
    {
        private readonly LeaveFilter _filter;

        public FilteredLeaveSpecification(LeaveFilter filter)
        {
            _filter = filter;
        }

        public Expression<Func<Leave, bool>>? Criteria =>
            leave =>
                (_filter.EmployeeId == null || leave.EmployeeId == _filter.EmployeeId) && 
                (_filter.Name == null || _filter.Name.ToLower().Contains(_filter.Name.ToLower())) &&
                (!_filter.Status.HasValue || (int) leave.LeaveStatus == _filter.Status) &&
                (_filter.Type == null ||  leave.LeaveType == _filter.Type) &&
                (!_filter.IsPaid.HasValue || leave.IsPaid == _filter.IsPaid) &&
                (!_filter.From.HasValue || leave.StartDate >= _filter.From.Value) &&
                (!_filter.To.HasValue || leave.EndDate <= _filter.To.Value) &&
                (!_filter.OnlyActiveRegisters || leave.IsActive);

        public List<string> Includes => ["Employee"];

        public bool IsTrackingEnabled => false;

        public Expression<Func<Leave, object>>? OrderBy =>
            !_filter.IsDesc ? GetOrderByExpression() : null;

        public Expression<Func<Leave, object>>? OrderByDescending =>
            _filter.IsDesc ? GetOrderByExpression() : null;

        public int? Skip => _filter.PageNumber;

        public int? Take => _filter.PageSize;

        private Expression<Func<Leave, object>> GetOrderByExpression()
        {
            return _filter.OrderBy switch
            {
                LeaveOrderBy.ByEmployeeName => leave => leave.Employee.FullName,
                LeaveOrderBy.ByStartDate => leave => leave.StartDate,
                LeaveOrderBy.ByEndDate => leave => leave.EndDate,
                LeaveOrderBy.ByStatus => leave => leave.LeaveStatus,
                LeaveOrderBy.ByType => leave => leave.LeaveType,
                _ => leave => leave.CreatedAt
            };
        }
    }
}
