using System.ComponentModel.DataAnnotations;
using LMS.Common.Results;
using MediatR;

namespace LMS.Application.Features.HR.Holidays.Commands.CreateHoliday
{
    public class CreateHolidayCommand : IRequest<Result>
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
