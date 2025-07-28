using LMS.Application.Abstractions.Authentication;
using LMS.Application.Abstractions.Common;
using LMS.Application.Abstractions.Loggings;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.Abstractions.UnitOfWorks;
using LMS.Common.Enums;
using LMS.Common.Results;
using LMS.Domain.HR.Models;
using MediatR;

namespace LMS.Application.Features.HR.Attendances.Commands.CheckOut
{
    public class CheckOutCommandHandler : IRequestHandler<CheckOutCommand, Result>
    {
        private readonly ISoftDeletableRepository<Employee> _employeeRepo;
        private readonly ISoftDeletableRepository<Attendance> _attendanceRepo;
        private readonly IAppLogger<CheckOutCommandHandler> _logger;
        private readonly IConverterHelper _converterHelper;
        private readonly IFaceRecognitionHelper _recognitionHelper;
        private readonly IUnitOfWork _unitOfWOrk;

        public CheckOutCommandHandler(
            ISoftDeletableRepository<Employee> employeeRepo,
            ISoftDeletableRepository<Attendance> attendanceRepo,
            IAppLogger<CheckOutCommandHandler> logger,
            IConverterHelper converterHelper,
            IFaceRecognitionHelper recognitionHelper,
            IUnitOfWork unitOfWork)
        {
            _employeeRepo = employeeRepo;
            _attendanceRepo = attendanceRepo;
            _logger = logger;
            _converterHelper = converterHelper;
            _recognitionHelper = recognitionHelper;
            _unitOfWOrk = unitOfWork;
        }

        public async Task<Result> Handle(CheckOutCommand request, CancellationToken cancellationToken)
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

                if (attendance.TimeOut is not null)
                {
                    return Result.Failure($"COMMON.{ResponseStatus.ALREADY_CHECKINED_OUT}");
                }

                if (attendance.TimeIn is null)
                {
                    return Result.Failure($"COMMON.{ResponseStatus.CANNAT_CHECK_OUT}");
                }

                var sortedFace = _converterHelper.ConvertBytesToDoubleArray(employee.FaceFeatureVector);

                var isMatch = _recognitionHelper.IsFaceMatching(request.FaceImage, sortedFace);

                if (!isMatch)
                {
                    return Result.Failure($"HR.AUTHENTICATION.{ResponseStatus.FACE_NOT_SAME}");
                }

                attendance.TimeOut = DateTime.UtcNow.TimeOfDay;

                await _unitOfWOrk.SaveChangesAsync();

                return Result.Success($"HR.ATTENDANCES.{ResponseStatus.CHECKING_SUCCESS}");
            } catch (Exception ex)
            {
                _logger.LogError("", ex);
                return Result.Failure($"HR.ATTENDANCES.{ResponseStatus.CANNAT_CHECK_OUT}");
            }
        }
    }
}
