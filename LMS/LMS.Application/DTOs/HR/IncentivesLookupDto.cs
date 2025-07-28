namespace LMS.Application.DTOs.HR
{
    public class IncentivesLookupDto
    {
        public decimal Amount { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime DecisionDate { get; set; }
        public bool IsPaid { get; set; }
    }
}
