using LMS.Application.Features.HR.Attendances.Commands.GenerateDailyAttendance;
using MediatR;
using Quartz;

namespace LMS.API.Services
{
    public class AttendanceGenerationJob : IJob
    {
        private readonly IMediator _mediator;

        public AttendanceGenerationJob(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _mediator.Send(new GenerateDailyAttendanceCommand());
        }
    }
}
