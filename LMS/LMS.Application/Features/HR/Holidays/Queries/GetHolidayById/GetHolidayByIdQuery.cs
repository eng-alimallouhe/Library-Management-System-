using LMS.Application.DTOs.HR;
using MediatR;

namespace LMS.Application.Features.HR.Holidays.Queries.GetHolidayById
{
    public record GetHolidayByIdQuery(Guid HolidayId) : IRequest<HolidayDetailsDto?>;
}