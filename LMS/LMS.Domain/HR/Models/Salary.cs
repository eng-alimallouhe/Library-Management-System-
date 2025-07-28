using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Domain.HR.Models
{
    public class Salary
    {
        public Guid SalaryId { get; set; }

        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        public int Year { get; set; }
        public int Month { get; set; }

        public decimal BaseSalary { get; set; }
        public decimal TotalIncentives { get; set; }
        public decimal TotalPenalties { get; set; }


        [NotMapped]
        public decimal NetSalary => BaseSalary + TotalIncentives - TotalPenalties;


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsPaid { get; set; }

        public string? PaymentReferenceUrl { get; set; }

        public bool IsActive { get; set; }

        public Salary()
        {
            SalaryId = Guid.NewGuid();
        }
    }
}
