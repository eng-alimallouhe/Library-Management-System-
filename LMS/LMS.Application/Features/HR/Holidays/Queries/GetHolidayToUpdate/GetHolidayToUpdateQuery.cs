using LMS.Application.DTOs.HR;
using MediatR;

namespace LMS.Application.Features.HR.Holidays.Queries.GetHolidayToUpdate
{
    public record GetHolidayToUpdateQuery(Guid HolidayId) : IRequest<HolidayToUpdateDto?>;
}