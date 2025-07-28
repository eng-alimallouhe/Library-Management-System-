using LMS.Domain.Financial.Enums;

namespace LMS.Application.DTOs.Financial
{
    public class RevenueDetailsDto
    {
        public Guid RevenueId { get; set; }
        public decimal Amount { get; set; }
        public string Date { get; set; }
        public Service Service { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
    }
}
