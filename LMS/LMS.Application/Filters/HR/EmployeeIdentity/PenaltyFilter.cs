using LMS.Application.Filters.Base;
namespace LMS.Application.Filters.HR.EmployeeIdentity
{
    public class PenaltyFilter : Filter
    {
        public List<Guid>? DepartmentIds { get; set; }
        public bool? ByIsApplied { get; set; }
        public bool? ByIsApproved { get; set; }

        public bool IsDesc { get; set; } = true;
        public PenaltyOrderBy OrderBy { get; set; }
    }
}
