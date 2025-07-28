using LMS.Common.Results;
using MediatR;

namespace LMS.Application.Features.HR.Holidays.Commands.UpdateHoliday
{
    public class UpdateHolidayCommand : IRequest<Result>
    {
        public Guid HolidayId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Notes { get; set; } = string.Empty;
        public DayOfWeek? Day { get; set; }
    }
}
