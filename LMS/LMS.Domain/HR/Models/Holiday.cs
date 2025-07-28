using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace LMS.Domain.HR.Models
{
    public class Holiday
    {
        [Key]
        public Guid HolidayId { get; set; }

        public string Name { get; set; }


        [AllowNull]
        public DateTime? StartDate { get; set; }

        [AllowNull]
        public DateTime? EndDate { get; set; }

        
        public bool IsWeeklyHoliday { get; set; } = false;

        public DayOfWeek? Day { get; set; }


        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string Notes { get; set; }

        public Holiday()
        {
            HolidayId = Guid.NewGuid();
            Name = string.Empty;
            Notes = string.Empty;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }

}
