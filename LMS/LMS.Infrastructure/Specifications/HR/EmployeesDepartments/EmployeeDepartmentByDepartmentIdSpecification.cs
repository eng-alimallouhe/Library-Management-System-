using System.Linq.Expressions;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.HR.Models;

namespace LMS.Infrastructure.Specifications.HR.EmployeesDepartments
{
    public class EmployeeDepartmentByDepartmentIdSpecification : ISpecification<EmployeeDepartment>
    {
        private readonly Guid _departmentId;

        public Expression<Func<EmployeeDepartment, bool>>? Criteria =>
            ed => ed.DepartmentId == _departmentId;

        public List<string> Includes => ["Employee", "Department"];

        public bool IsTrackingEnabled => false;

        public Expression<Func<EmployeeDepartment, object>>? OrderBy => null;

        public Expression<Func<EmployeeDepartment, object>>? OrderByDescending => null;

        public int? Skip => null;

        public int? Take => null;


        public EmployeeDepartmentByDepartmentIdSpecification(
            Guid departmentId)
        {
            _departmentId = departmentId;
        }
    }
}
