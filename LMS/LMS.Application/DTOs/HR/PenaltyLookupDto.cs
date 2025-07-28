using LMS.Domain.HR.Models;

namespace LMS.Application.DTOs.HR
{
    public class PenaltyLookupDto
    {
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public DateTime DecisionDate { get; set; }
        public bool IsApplied { get; set; }
    }
}
