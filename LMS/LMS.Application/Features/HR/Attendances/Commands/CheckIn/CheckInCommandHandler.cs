using LMS.Application.Abstractions;
using LMS.Application.Abstractions.Authentication;
using LMS.Application.Abstractions.Common;
using LMS.Application.Abstractions.Loggings;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.Abstractions.UnitOfWorks;
using LMS.Common.Enums;
using LMS.Common.Results;
using LMS.Domain.HR.Models;
using MediatR;

namespace LMS.Application.Features.HR.Attendances.Commands.CheckIn
{
    public class CheckInCommandHandler : IRequestHandler<CheckInCommand, Result>
    {
        private readonly ISoftDeletableRepository<Employee> _employeeRepo;
        private readonly ISoftDeletableRepository<Attendance> _attendanceRepo;
        private readonly IAppLogger<CheckInCommandHandler> _logger;
        private readonly IConverterHelper _converterHelper;
        private readonly IFaceRecognitionHelper _recognitionHelper;
        private readonly IUnitOfWork _unitOfWork;

        public CheckInCommandHandler(
            ISoftDeletableRepository<Employee> employeeRepo,
            ISoftDeletableRepository<Attendance> attendanceRepo,
            IAppLogger<CheckInCommandHandler> logger,
            IConverterHelper converterHelper,
            IFaceRecognitionHelper recognitionHelper,
            IUnitOfWork unitOfWork)
        {
            _employeeRepo = employeeRepo;
            _attendanceRepo = attendanceRepo;
            _logger = logger;
            _converterHelper = converterHelper;
            _recognitionHelper = recognitionHelper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(CheckInCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var employee = await _employeeRepo.GetByExpressionAsync(e => e.UserName == request.UserName, false);
                
                if (employee == null)
                {
                    return Result.Failure($"AUTHENTICATION.{ResponseStatus.ACCOUNT_NOT_FOUND}");
                }

                var today = DateOnly.FromDateTime(DateTime.UtcNow);

                var attendance = await _attendanceRepo.GetByExpressionAsync(a => 
                    a.EmployeeId == employee.UserId && 
                    DateOnly.FromDateTime(a.Date) == today, true
                );

                if (attendance is null)
                {
                    return Result.Failure($"COMMON.{ResponseStatus.RECORD_NOT_FOUND}");
                }

                var sortedFace = _converterHelper.ConvertBytesToDoubleArray(employee.FaceFeatureVector);

                var isMatch = _recognitionHelper.IsFaceMatching(request.FaceImage, sortedFace);

                if (!isMatch)
                {
                    return Result.Failure($"HR.AUTHENTICATION.{ResponseStatus.FACE_NOT_SAME}");
                }

                attendance.TimeIn = DateTime.UtcNow.TimeOfDay;

                DateTime.UtcNow.ConvertToSyrianTime();

                await _unitOfWork.SaveChangesAsync();

                return Result.Success($"HR.ATTENDANCES.{ResponseStatus.CHECKING_SUCCESS}");
            } catch (Exception ex)
            {
                _logger.LogError("", ex);
                return Result.Failure($"HR.ATTENDANCES.{ResponseStatus.FACE_NOT_SAME}");
            }
        }
    }
}
