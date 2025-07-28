using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.HR;
using LMS.Application.Filters.HR.EmployeeIdentity;
using MediatR;

namespace LMS.Application.Features.HR.Attendances.Queries.GetAllAttendances
{
    public record GetAllAttendancesQuery(
        AttendanceFilter Filter) : IRequest<PagedResult<AttendanceOverviewDto>>;
}
