namespace LMS.API.DTOs.HR
{
    public class LeaveCreateRequestDto
    {
        public Guid EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int LeaveType { get; set; }
        public string Reason { get; set; }
    }
}
