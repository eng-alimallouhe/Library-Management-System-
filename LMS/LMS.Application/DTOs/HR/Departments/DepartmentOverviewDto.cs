using LMS.Domain.HR.Models;

namespace LMS.Application.DTOs.HR.Departments
{
    public class DepartmentOverviewDto
    {
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string DepartmentDescription { get; set; } = string.Empty;
        public int EmployeesCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}