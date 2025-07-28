using System.Linq.Expressions;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.HR.Models;

namespace LMS.Infrastructure.Specifications.HR.EmployeesDepartments
{
    public class AvaliableDepartmentsSpecification : ISpecification<Department>
    {
        private readonly Guid? _employeeId;

        public Expression<Func<Department, bool>>? Criteria => 
            d => (_employeeId == null) ||
            d.EmployeeDepartments.Any(ed => ed.EmployeeId == _employeeId && ed.IsActive);


        public List<string> Includes => ["EmployeeDepartments"];

        public bool IsTrackingEnabled => false;

        public Expression<Func<Department, object>>? OrderBy => null;

        public Expression<Func<Department, object>>? OrderByDescending => null;

        public int? Skip => null;

        public int? Take => null;

        public AvaliableDepartmentsSpecification(
            Guid? employeeId)
        {
            _employeeId = employeeId;            
        }
    }
}
