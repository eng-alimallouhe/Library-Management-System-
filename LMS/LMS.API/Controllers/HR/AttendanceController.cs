using AutoMapper;
using LMS.API.DTOs.HR;
using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.HR;
using LMS.Application.Features.HR.Attendances.Commands.CheckIn;
using LMS.Application.Features.HR.Attendances.Commands.CheckOut;
using LMS.Application.Features.HR.Attendances.Commands.DeleteAttendance;
using LMS.Application.Features.HR.Attendances.Queries.GetAllAttendances;
using LMS.Application.Features.HR.Holidays.Queries.GetAllHolidays;
using LMS.Application.Filters.HR.EmployeeIdentity;
using LMS.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers.HR
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/hr/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;


        public AttendanceController(
            IMediator mediator,
            IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [MapToApiVersion("1.0")]
        [HttpGet]
        public async Task<ActionResult<PagedResult<AttendanceOverviewDto>>> GetAllAttendancesAsync([FromQuery] AttendanceFilter filter)
        {
            var response = await _mediator.Send(new GetAllAttendancesQuery(filter));
            return Ok(response);
        }


        [MapToApiVersion("1.0")]
        [HttpPut("check-in")]
        public async Task<ActionResult<Result>> CheckInAsync([FromForm] CheckDto check)
        {
            using var stream = check.FaceImage.OpenReadStream();
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var faceByByte = memoryStream.ToArray();

            var command = new CheckInCommand
            {
                FaceImage = faceByByte,
                UserName = check.UserName
            };

            var response = await _mediator.Send(command);

            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }



        [MapToApiVersion("1.0")]
        [HttpPut("check-out")]
        public async Task<ActionResult<Result>> CheckOutAsync([FromForm] CheckDto check)
        {
            using var stream = check.FaceImage.OpenReadStream();
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var faceByByte = memoryStream.ToArray();

            var command = new CheckOutCommand
            {
                FaceImage = faceByByte,
                UserName = check.UserName
            };

            var response = await _mediator.Send(command);

            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }



        [MapToApiVersion("1.0")]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<Result>> DeleteAttendanceAsync(Guid id)
        {
            var command = new DeleteAttendanceCommand
            {
                AttendanceId = id
            };

            var response = await _mediator.Send(command);

            return response.IsSuccess? Ok(response) : BadRequest(response);
        }
    }
}
