using LMS.Domain.HR.Enums;

namespace LMS.Application.DTOs.HR
{
    public class LeaveUpdateDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public LeaveType LeaveType { get; set; }
        public required string Reason { get; set; }
    }
}
