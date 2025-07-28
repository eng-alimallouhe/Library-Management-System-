using LMS.Application.Filters.Base;
using LMS.Domain.HR.Enums;

namespace LMS.Application.Filters.HR.EmployeeIdentity
{
    public class LeaveFilter : Filter
    {
        public Guid? EmployeeId { get; set; } = null;
        public int? Status { get; set; }
        public bool? IsPaid { get; set; }
        public bool IsDesc { get; set; } = true;
        public LeaveType? Type { get; set; }
        public LeaveOrderBy OrderBy { get; set; } = LeaveOrderBy.ByCreatedAt;
    }
}