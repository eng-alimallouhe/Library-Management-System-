using System.Linq.Expressions;
using LMS.Application.Filters.HR;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.HR.Models;

namespace LMS.Infrastructure.Specifications.HR.EmployeesDepartments
{
    public class EmployeesFilteredSpecification : ISpecification<Employee>
    {
        private readonly EmployeeFilter _filter;

        public Expression<Func<Employee, bool>>? Criteria => 
            e => 
            (_filter.Name == null || e.FullName.ToLower().Contains(_filter.Name.ToLower())) && 
            
            (_filter.From == null || _filter.From <= e.HireDate) &&
            
            (_filter.To == null || e.HireDate <= _filter.To) &&

            (!_filter.OnlyActiveRegisters || !e.IsDeleted && !e.IsLocked) && 
            
            (_filter.DepartmentIds == null || 
             e.EmployeeDepartments.Any(ed => ed.IsActive &&
                                         _filter.DepartmentIds.Contains(ed.DepartmentId)));

        public List<string> Includes => ["EmployeeDepartments.Department"];

        public bool IsTrackingEnabled => false;

        public Expression<Func<Employee, object>>? OrderBy => e => e.CreatedAt;

        public Expression<Func<Employee, object>>? OrderByDescending => null;

        public int? Skip => _filter.PageNumber;

        public int? Take => _filter.PageSize;


        public EmployeesFilteredSpecification(
            EmployeeFilter filter)
        {
            _filter = filter;
        }
    }
}
