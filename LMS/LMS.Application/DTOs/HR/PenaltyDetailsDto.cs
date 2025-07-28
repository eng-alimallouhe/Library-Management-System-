namespace LMS.Application.DTOs.HR
{
    public class PenaltyDetailsDto
    {
        public Guid PenaltyId { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime DecisionDate { get; set; }
        public string DecisionFileUrl { get; set; } = string.Empty;
    }
}