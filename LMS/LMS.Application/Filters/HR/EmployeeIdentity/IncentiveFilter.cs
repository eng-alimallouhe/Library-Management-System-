using LMS.Application.Filters.Base;

namespace LMS.Application.Filters.HR.EmployeeIdentity
{
    public class IncentiveFilter : Filter
    {
        public List<Guid>? DepartmentIds { get; set; }
        public bool? ByIsPaid { get; set; }
        public bool? ByIsApproved { get; set; }

        public bool IsDesc { get; set; } = true;
        public IncentiveOrderBy OrderBy { get; set; }
    }
}
