namespace LMS.Application.DTOs.HR
{
    public class AttendanceOverviewDto
    {
        public Guid AttendanceId { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;    
        public string? TimeIn { get; set; }                 
        public string? TimeOut { get; set; }                
    }
}
