using LMS.Common.Results;
using MediatR;

namespace LMS.Application.Features.HR.Attendances.Commands.DeleteAttendance
{
    public class DeleteAttendanceCommand : IRequest<Result>
    {
        public Guid AttendanceId { get; set; }
    }
}
