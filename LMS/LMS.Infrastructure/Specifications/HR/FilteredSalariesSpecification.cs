using System.Linq.Expressions;
using LMS.Application.Filters.Finacial;
using LMS.Application.Filters.HR;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.HR.Models;

namespace LMS.Infrastructure.Specifications.HR
{
    public class FilteredSalariesSpecification : ISpecification<Salary>
    {
        private readonly SalaryFilter _filter;

        public FilteredSalariesSpecification(SalaryFilter filter)
        {
            _filter = filter;
        }

        public Expression<Func<Salary, bool>>? Criteria =>
        s =>
            (_filter.DepartmentIds == null || !_filter.DepartmentIds.Any() ||
            s.Employee.EmployeeDepartments
            .Any(ed => ed.IsActive && _filter.DepartmentIds.Contains(ed.DepartmentId))) &&
            
            (_filter.EmployeeIds == null || !_filter.EmployeeIds.Any() || 
            _filter.EmployeeIds.Contains(s.EmployeeId)) &&
            
            (_filter.IsPaid == null || s.IsPaid == _filter.IsPaid) &&
            (_filter.Month == null || s.Month == _filter.Month) &&
            (_filter.Year == null || s.Year == _filter.Year) &&
            (_filter.From == null || s.CreatedAt >= _filter.From) &&
            (_filter.To == null || s.CreatedAt <= _filter.To) &&
            (_filter.Name == null || s.Employee.FullName.Contains(_filter.Name));


        public List<string> Includes => ["Employee.EmployeeDepartments"];

        public bool IsTrackingEnabled => false;

        public Expression<Func<Salary, object>>? OrderBy =>
            _filter.IsDesc ? null : GetOrderingExpression(_filter.OrderBy);

        public Expression<Func<Salary, object>>? OrderByDescending =>
            _filter.IsDesc ? GetOrderingExpression(_filter.OrderBy) : null;

        public int? Skip => _filter.PageNumber;
        public int? Take => _filter.PageSize;

        private Expression<Func<Salary, object>> GetOrderingExpression(SalaryOrdering? order)
        {
            return order switch
            {
                SalaryOrdering.EmployeeName => s => s.Employee.FullName,
                SalaryOrdering.NetSalary => s => s.BaseSalary + s.TotalIncentives - s.TotalPenalties,
                SalaryOrdering.Date => s => s.CreatedAt,
                _ => s => s.CreatedAt
            };
        }
    }
}