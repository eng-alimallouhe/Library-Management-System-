﻿namespace LMS.Domain.HR.Models
{
    public class Department
    {
        //primary key
        public Guid DepartmentId { get; set; }


        public required string DepartmentName { get; set; } 
        public required string DepartmentDescription { get; set; }


        //soft delete:
        public bool IsActive { get; set; } 


        //Timestamp:
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; } 


        //navigation property:
        public ICollection<EmployeeDepartment> EmployeeDepartments { get; set; }


        public Department()
        {
            DepartmentId = Guid.NewGuid();
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            EmployeeDepartments = [];
        }
    }
}
