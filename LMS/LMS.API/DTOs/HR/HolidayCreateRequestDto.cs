using System.ComponentModel.DataAnnotations;

namespace LMS.API.DTOs.HR
{
    public class HolidayCreateRequestDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(512)]
        public string Notes { get; set; } = string.Empty;

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DayOfWeek? Day { get; set; }
    }
}
