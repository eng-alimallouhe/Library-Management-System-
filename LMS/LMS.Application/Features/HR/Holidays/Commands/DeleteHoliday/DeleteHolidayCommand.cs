using LMS.Common.Results;
using MediatR;

namespace LMS.Application.Features.HR.Holidays.Commands.DeleteHoliday
{
    public class DeleteHolidayCommand : IRequest<Result>
    {
        public Guid HolidayId { get; set; }
    }
}
