namespace LMS.Application.DTOs.HR
{
    public class HolidayOverviewDto
    {
        public Guid HolidayId { get; set; }
        public string Name { get; set; } = default!;
        public string StartDate { get; set; } = default!;
        public string EndDate { get; set; } = default!;
        public string Notes { get; set; } = default!;
    }
}