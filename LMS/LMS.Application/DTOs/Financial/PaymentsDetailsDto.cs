namespace LMS.Application.DTOs.Financial
{
    public class PaymentsDetailsDto
    {
        public Guid PaymentId { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Reasone { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
    }
}
