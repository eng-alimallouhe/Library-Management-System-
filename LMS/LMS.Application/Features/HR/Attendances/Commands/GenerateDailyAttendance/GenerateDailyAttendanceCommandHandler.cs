using LMS.Application.Abstractions;
using LMS.Application.Abstractions.Loggings;
using LMS.Application.Abstractions.Repositories;
using LMS.Common.Results;
using LMS.Domain.HR.Models;
using MediatR;

namespace LMS.Application.Features.HR.Attendances.Commands.GenerateDailyAttendance
{
    public class GenerateDailyAttendanceCommandHandler : IRequestHandler<GenerateDailyAttendanceCommand, Result>
    {
        private readonly ISoftDeletableRepository<Employee> _employeeRepo;
        private readonly IBaseRepository<Holiday> _holidayRepo;
        private readonly ISoftDeletableRepository<Attendance> _attendanceRepo;
        private readonly IAppLogger<GenerateDailyAttendanceCommandHandler> _logger;

        public GenerateDailyAttendanceCommandHandler(
            ISoftDeletableRepository<Employee> employeeRepo,
            IBaseRepository<Holiday> holidayRepo,
            ISoftDeletableRepository<Attendance> attendanceRepo,
            IAppLogger<GenerateDailyAttendanceCommandHandler> logger)
        {
            _employeeRepo = employeeRepo;
            _holidayRepo = holidayRepo;
            _attendanceRepo = attendanceRepo;
            _logger = logger;
        }

        public async Task<Result> Handle(GenerateDailyAttendanceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var date = DateTime.UtcNow;

                var nowDay = date.DayOfWeek;

                var holidays = await _holidayRepo.GetAllByExpressionAsync(h => true);

                bool isHoliday = holidays.Any(h =>
                    h.IsWeeklyHoliday && h.Day == nowDay ||
                    !h.IsWeeklyHoliday && h.StartDate <= date && date <= h.EndDate
                );

                if (isHoliday)
                {
                    return Result.Failure("HOLIDAY");
                }

                var employees = await _employeeRepo.GetAllByExpressionAsync(emp => !emp.IsDeleted);

                var attendances = new List<Attendance>();

                foreach (var item in employees)
                {
                    attendances.Add(new Attendance
                    {
                        EmployeeId = item.UserId,
                        Date = DateTime.UtcNow,
                        TimeIn = null,
                        TimeOut = null
                    });
                }

                await _attendanceRepo.AddRangeAsync(attendances);

                return Result.Success("SUCCESS");

            } catch (Exception ex)
            {
                _logger.LogError($"Error while generating the daily attendences for the date {DateTime.UtcNow.ConvertToSyrianTime()}", ex);
                return Result.Failure(ex.Message);
            }
        }
    }
}
