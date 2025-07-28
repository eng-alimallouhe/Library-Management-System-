using LMS.Domain.Financial.Enums;

namespace LMS.Application.DTOs.Financial
{
    public class RevenueOverviewDto
    {
        public Guid RevenueId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid CustomerId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public Service Service { get; set; }
    }
}
