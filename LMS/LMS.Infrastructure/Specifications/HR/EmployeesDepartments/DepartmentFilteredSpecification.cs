using System.Linq.Expressions;
using LMS.Application.Filters.HR;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.HR.Models;

namespace LMS.Infrastructure.Specifications.HR.EmployeesDepartments
{
    public class DepartmentFilteredSpecification : ISpecification<Department>
    {
        private readonly DepartmentFilter _filter;

        public Expression<Func<Department, bool>>? Criteria =>
            d =>
            (_filter.Name == null || d.DepartmentName.ToLower().Contains(_filter.Name.ToLower())) && 
            (_filter.From == null || d.CreatedAt >= _filter.From) &&
            (!_filter.OnlyActiveRegisters || d.IsActive) &&
            (_filter.From == null || d.CreatedAt >= _filter.From) &&
            (_filter.To == null || d.CreatedAt <= _filter.To) &&
            (_filter.MaxEmployeeCount == null || d.EmployeeDepartments.Count(ed => ed.IsActive) <= _filter.MaxEmployeeCount) &&
            (_filter.MinEmployeeCount == null || d.EmployeeDepartments.Count(ed => ed.IsActive) >= _filter.MinEmployeeCount);


        public List<string> Includes => ["EmployeeDepartments"];

        public bool IsTrackingEnabled => false;

        public Expression<Func<Department, object>>? OrderBy => null;

        public Expression<Func<Department, object>>? OrderByDescending => null;

        public int? Skip => _filter.PageNumber == null? null : _filter.PageNumber; 

        public int? Take => _filter.PageSize == null ? null : _filter.PageSize;


        public DepartmentFilteredSpecification(
            DepartmentFilter filter)
        {
            _filter = filter;
        }
    }
}
