using LMS.Application.Filters.Base;

namespace LMS.Application.Filters.HR
{
    public class DepartmentFilter : Filter
    {
        public int? MaxEmployeeCount { get; set; }
        public int? MinEmployeeCount { get; set; }
    }
}
