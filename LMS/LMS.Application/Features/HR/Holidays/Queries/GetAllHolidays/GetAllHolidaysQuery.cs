using LMS.Application.DTOs.HR;
using MediatR;

namespace LMS.Application.Features.HR.Holidays.Queries.GetAllHolidays
{
    public record GetAllHolidaysQuery : IRequest<ICollection<HolidayOverviewDto>>;
}