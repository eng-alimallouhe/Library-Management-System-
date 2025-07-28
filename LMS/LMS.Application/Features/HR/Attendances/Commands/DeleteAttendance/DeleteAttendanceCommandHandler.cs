using LMS.Application.Abstractions.Repositories;
using LMS.Common.Enums;
using LMS.Common.Exceptions;
using LMS.Common.Results;
using LMS.Domain.HR.Models;
using MediatR;

namespace LMS.Application.Features.HR.Attendances.Commands.DeleteAttendance
{
    public class DeleteAttendanceCommandHandler : IRequestHandler<DeleteAttendanceCommand, Result>
    {
        private readonly ISoftDeletableRepository<Attendance> _attendanceRepo;

        public DeleteAttendanceCommandHandler(
            ISoftDeletableRepository<Attendance> attendanceRepo)
        {
            _attendanceRepo = attendanceRepo;
        }

        public async Task<Result> Handle(DeleteAttendanceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _attendanceRepo.SoftDeleteAsync(request.AttendanceId);

                return Result.Success($"COMMON.{ResponseStatus.DELETE_COMPLETED}");
            }
            catch (EntityNotFoundException)
            {
                return Result.Failure($"COMMON.{ResponseStatus.RECORD_NOT_FOUND}");
            }
            catch (Exception)
            {
                return Result.Failure($"COMMON.{ResponseStatus.UNABLE_DELETE_ELEMENT}");
            }
        }
    }
}
