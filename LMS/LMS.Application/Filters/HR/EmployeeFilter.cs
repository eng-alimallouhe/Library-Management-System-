using LMS.Application.Filters.Base;

namespace LMS.Application.Filters.HR
{
    public class EmployeeFilter : Filter
    {
        public ICollection<Guid>? DepartmentIds { get; set; }
    }
}