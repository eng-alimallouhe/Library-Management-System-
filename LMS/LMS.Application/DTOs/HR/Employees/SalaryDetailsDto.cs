using LMS.Domain.HR.Models;

namespace LMS.Application.DTOs.HR.Employees
{
    public class SalaryDetailsDto
    {
        public Guid SalaryId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid DepartmentId { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentName { get; set; }
        public string Date { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal TotalIncentives { get; set; }
        public decimal TotalPenalties { get; set; }
        public decimal NetSalaryValue => BaseSalary + TotalIncentives - TotalPenalties;
        public bool IsPaid { get; set; }
    }
}
