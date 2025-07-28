namespace LMS.Application.DTOs.HR
{
    public class IncentiveOverviewDto
    {
        public Guid IncentiveId { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public bool IsPaid { get; set; }
        public bool IsApproved { get; set; }
    }
}