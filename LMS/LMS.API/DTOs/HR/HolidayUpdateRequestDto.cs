namespace LMS.API.DTOs.HR
{
    public class HolidayUpdateRequestDto
    {
        public Guid HolidayId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Notes { get; set; } = string.Empty;
        public DayOfWeek? Day { get; set; }
    }
}
