using LMS.Domain.HR.Enums;

namespace LMS.Application.DTOs.HR
{
    public class LeaveOverviewDto
    {
        public Guid LeaveId { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = default!;
        public DateTime StartDate { get; set; } = default!;
        public DateTime EndDate { get; set; } = default!;
        public LeaveType LeaveType { get; set; }
        public LeaveStatus LeaveStatus { get; set; }
        public bool IsPaid { get; set; }
    }
}
