using LMS.Domain.Customers.Models;
using LMS.Domain.Financial.Enums;
using LMS.Domain.HR.Models;

namespace LMS.Domain.Financial.Models
{
    public class Revenue
    {
        //Primary Key:
        public Guid RevenueId { get; set; }


        //Foreign Key: CustomerId ==> one(Customer)-to-many(Payment) relationship
        public Guid CustomerId { get; set; }


        //Foreign Key: EmployeeId ==> one(Employee)-to-many(PrintOrder) relationship
        public Guid EmployeeId { get; set; }


        public decimal Amount { get; set; }
        public Service Service { get; set; }


        //Soft Delete:
        public bool IsActive { get; set; }


        //Timestamp:
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


        //Navigation Property: 
        public Customer Customer { get; set; }
        public Employee Employee { get; set; }

        public Revenue()
        {
            RevenueId = Guid.NewGuid();
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            Customer = null!;
            Employee = null!;
        }
    }
}
