using LMS.Domain.HR.Models;

namespace LMS.Application.DTOs.HR
{
    public class PenaltyOverviewDto
    {
        public Guid PenaltyId { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime DecisionDate { get; set; }
        public bool IsApplied { get; set; }
        public bool IsApproved { get; set; }
    }
}
