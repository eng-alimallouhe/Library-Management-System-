using LMS.Application.Filters.Base;

namespace LMS.Application.Filters.HR
{
    public class SalaryFilter : Filter
    {
        public List<Guid>? DepartmentIds { get; set; }
        public List<Guid>? EmployeeIds { get; set; }
        public bool? IsPaid { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public SalaryOrdering? OrderBy { get; set; }
        public bool IsDesc { get; set; } = false;
    }
}
