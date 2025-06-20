using LMS.Domain.Financial.Models;
using LMS.Domain.Identity.Models;
using LMS.Domain.Orders.Models;
using Microsoft.VisualBasic;

namespace LMS.Domain.HR.Models
{
    public class Employee : User
    {
        public DateTime HireDate { get; set; }
        public decimal BaseSalary { get; set; }


        //Navigation Property:
        public ICollection<EmployeeDepartment> EmployeeDepartments { get; set; }
        public ICollection<Attendance> Attendances { get; set; }  
        public ICollection<Incentive> Incentives { get; set; }  
        public ICollection<Penalty> Penalties { get; set; }  
        public ICollection<Leave> Leaves { get; set; }  
        public ICollection<Salary> Salaries { get; set; }  
        public LeaveBalance LeaveBalance { get; set; }
        public ICollection<BaseOrder> Orders { get; set; }
        public ICollection<Revenue> Revenues { get; set; }


        public Employee()
        {
            EmployeeDepartments = [];
            Attendances = [];
            Incentives = [];
            Penalties = [];
            Leaves = [];
            Salaries = [];
            Orders = [];
            Revenues = [];
            LeaveBalance = null!;
        }
    }
}