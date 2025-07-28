namespace LMS.Application.DTOs.HR.Departments
{
    public class DepartmentHistoryDto
    {
        public string DepartmentName { get; set; } = string.Empty;
        public DateTime JoinDate { get; set; }
        public DateTime? LeaveDate { get; set; }
        public bool IsCurrent { get; set; }
        public bool IsManager { get; set; }
    }
}
