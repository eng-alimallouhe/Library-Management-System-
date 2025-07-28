namespace LMS.Application.DTOs.Reports
{
    public class PaymentReportDto
    {
        public Guid PaymentId { get; set; }
        public string EmployeeName { get; set; } = default!;
        public decimal Amount { get; set; }
        public string Details { get; set; } = default!;
        public string Date { get; set; } = default!;
    }
}
