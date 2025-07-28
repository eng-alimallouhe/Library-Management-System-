namespace LMS.Domain.HR.Models
{
    public class Incentive
    {
        //primary key
        public Guid IncentiveId { get; set; }

        
        //Foreign Key: EmployeeId ==> one(employee)-to-many(incentives) relationship
        public Guid EmployeeId { get; set; }


        public decimal Amount { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string DecisionFileUrl { get; set; } = string.Empty;
        public DateTime DecisionDate { get; set; }
        public bool IsPaid { get; set; }
        public bool IsApproved { get; set; } = false;


        //Soft Delete
        public bool IsActive { get; set; }


        //Navigation Property:
        public Employee Employee { get; set; }


        public Incentive()
        {
            IncentiveId = Guid.NewGuid();
            IsActive = true;
            Employee = null!;
        }
    }
}