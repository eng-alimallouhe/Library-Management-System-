using LMS.Domain.Financial.Enums;

namespace LMS.Application.DTOs.Reports
{
    public class RevenueReportDto
    {
        public Guid FinancialId { get; set; }
        public string EmployeeName { get; set; } = default!;
        public string CustomerName { get; set; } = default!;
        public decimal Amount { get; set; }
        public Service Service { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}