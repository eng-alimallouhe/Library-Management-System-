using System.Linq.Expressions;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.HR.Models;

namespace LMS.Infrastructure.Specifications.HR.EmployeesDepartments
{
    public class EmployeeDepartmentByEmployeeIdSpecification : ISpecification<EmployeeDepartment>
    {
        private readonly Guid _employeeId;
        private readonly bool _isActive;
        private readonly bool _includeDepartment;


        public Expression<Func<EmployeeDepartment, bool>>? Criteria => 
            ed => ed.EmployeeId == _employeeId &&
            (!_isActive || ed.IsActive);

        public List<string> Includes => (_includeDepartment)? ["Department"] : [];

        public bool IsTrackingEnabled => false;

        public Expression<Func<EmployeeDepartment, object>>? OrderBy => null;

        public Expression<Func<EmployeeDepartment, object>>? OrderByDescending => null;

        public int? Skip => null;

        public int? Take => null;


        public EmployeeDepartmentByEmployeeIdSpecification(
            Guid employeeId,
            bool isActive = false,
            bool includeDepartment = true)
        {
            _employeeId = employeeId;
            _isActive = isActive;
            _includeDepartment = includeDepartment;
        }
    }
}
