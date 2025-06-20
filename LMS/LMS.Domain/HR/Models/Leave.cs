using LMS.Domain.HR.Enums;

namespace LMS.Domain.HR.Models
{
    public class Leave
    {
        //Primary key
        public Guid LeaveId { get; set; }

        
        //Foreign Key: EmployeeId ==> one(employee)-to-many(leaves) relationship
        public Guid EmployeeId { get; set; }


        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public LeaveType? LeaveType { get; set; }
        public LeaveStatus LeaveStatus { get; set; }
        public required string Reason { get; set; }


        //Soft Delete
        public bool IsActive { get; set; }


        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        //Navigation Property:
        public Employee Employee { get; set; }

        public Leave()
        {
            LeaveId = Guid.NewGuid();
            IsActive = true;
            Employee = null!;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
    }
}
