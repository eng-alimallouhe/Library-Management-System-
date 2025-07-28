namespace LMS.Domain.HR.Models
{
    public class EmployeeDepartment
    {
        //primary key
        public Guid EmployeeDepartmentId { get; set; }

        
        //Foreign Key: EmployeeId ==> one(employee)-to-many(employeeDepartments) relationship
        public Guid EmployeeId { get; set; }

        
        //Foreign Key: DepartmentId ==> one(department)-to-many(employeeDepartments) relationship
        public Guid DepartmentId { get; set; }

        
        public required string AppointmentDecisionUrl { get; set; }

        public bool IsManager { get; set; } = false;

        //Soft Delete:
        public bool IsActive { get; set; }

        
        //Timestamp:
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; } 

        
        //Navigation Property:
        public Employee Employee { get; set; }
        public Department Department { get; set; } 

        public EmployeeDepartment()
        {
            EmployeeDepartmentId = Guid.NewGuid();
            StartDate = DateTime.UtcNow;
            IsActive = true;
            Employee = null!;
            Department = null!;
        }
    }
}
