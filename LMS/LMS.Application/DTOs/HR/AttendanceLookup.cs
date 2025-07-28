namespace LMS.Application.DTOs.HR
{
    public class AttendanceLookup
    {
        public DateTime Date { get; set; }
        public TimeSpan? TimeIn { get; set; }
        public TimeSpan? TimeOut { get; set; }
        public int Day {  get; set; }
    }
}