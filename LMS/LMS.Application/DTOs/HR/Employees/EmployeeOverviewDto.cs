namespace LMS.Application.DTOs.HR.Employees
{
    public class EmployeeOverviewDto
    {
        public Guid EmployeeId { get; set; }
        public string FullName { get; set; }   = string.Empty;
        public string HireDate { get; set; }
        public string? CurrentDepartmentName { get; set; } = string.Empty;
    }
}
