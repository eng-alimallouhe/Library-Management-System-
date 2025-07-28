using LMS.Domain.HR.Enums;

namespace LMS.API.DTOs.HR
{
    public class LeaveUpdateRequestDto
    {
        public Guid EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public LeaveType LeaveType { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
