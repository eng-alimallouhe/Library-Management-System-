using System.Linq.Expressions;
using LMS.Application.Filters.HR.EmployeeIdentity;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.HR.Models;

namespace LMS.Infrastructure.Specifications.HR.EmployeeIdentity
{
    public class FilteredAttendanceSpecification : ISpecification<Attendance>
    {
        private readonly AttendanceFilter _filter;

        public FilteredAttendanceSpecification(AttendanceFilter filter)
        {
            _filter = filter;
        }

        
        public Expression<Func<Attendance, bool>>? Criteria =>
            a =>
                (_filter.EmployeeIds == null || !_filter.EmployeeIds.Any() || _filter.EmployeeIds.Contains(a.EmployeeId)) &&

                (_filter.IsCheckedIn == null || (_filter.IsCheckedIn.Value ? a.TimeIn != null : a.TimeIn == null)) &&

                (_filter.IsCheckedOut == null || (_filter.IsCheckedOut.Value ? a.TimeOut != null : a.TimeOut == null)) &&

                (_filter.From == null || a.Date >= _filter.From) &&

                (_filter.To == null || a.Date <= _filter.To);

        
        public List<string> Includes => new List<string> { "Employee" };

        
        public bool IsTrackingEnabled => false;

        
        public Expression<Func<Attendance, object>>? OrderBy => !_filter.IsDesc ? GetOrderExpression(_filter.OrderBy) : null;

        
        public Expression<Func<Attendance, object>>? OrderByDescending => _filter.IsDesc ? GetOrderExpression(_filter.OrderBy) : null;

        
        public int? Skip => _filter.PageNumber;

        
        public int? Take => _filter.PageSize;

        
        private Expression<Func<Attendance, object>> GetOrderExpression(AttendanceOrderBy orderBy)
        {
            return orderBy switch
            {
                AttendanceOrderBy.ByEmployeeName => a => a.Employee.FullName!,
                AttendanceOrderBy.ByDate => a => a.Date,
                AttendanceOrderBy.ByTimeIn => a => a.TimeIn ?? TimeSpan.Zero,
                AttendanceOrderBy.ByTimeOut => a => a.TimeOut ?? TimeSpan.Zero,
                _ => a => a.Date,
            };
        }
    }
}
