using System.Linq.Expressions;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.HR.Models;

namespace LMS.Infrastructure.Specifications.HR.EmployeesDepartments
{
    public class EmployeeDetailsSpecification : ISpecification<Employee>
    {
        private readonly Guid _id;

        public Expression<Func<Employee, bool>>? Criteria =>
                employee => employee.UserId == _id;

        public List<string> Includes => [
            "Revenues",
            "Salaries",
            "Incentives",
            "Penalties",
            "Leaves",
            "LeaveBalance",
            "EmployeeDepartments.Department"];

        public bool IsTrackingEnabled => false;

        public Expression<Func<Employee, object>>? OrderBy => null;

        public Expression<Func<Employee, object>>? OrderByDescending => null;

        public int? Skip => null;

        public int? Take => null;

        public EmployeeDetailsSpecification(Guid id)
        {
            _id = id;
        }
    }
}
