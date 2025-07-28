using AutoMapper;
using LMS.API.DTOs.HR;
using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.HR;
using LMS.Application.Features.HR.Holidays.Commands.CreateHoliday;
using LMS.Application.Features.HR.Holidays.Commands.DeleteHoliday;
using LMS.Application.Features.HR.Holidays.Commands.UpdateHoliday;
using LMS.Application.Features.HR.Holidays.Queries.GetAllHolidays;
using LMS.Application.Features.HR.Holidays.Queries.GetHolidayById;
using LMS.Application.Features.HR.Holidays.Queries.GetHolidayToUpdate;
using LMS.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers.HR
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/hr/[controller]")]
    [ApiController]
    public class HolidayController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;


        public HolidayController(
            IMediator mediator,
            IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }


        [MapToApiVersion("1.0")]
        [HttpGet]
        public async Task<ActionResult<ICollection<HolidayOverviewDto>>> GetAllHolidayesAsync()
        {
            var response = await _mediator.Send(new GetAllHolidaysQuery());
            return Ok(response);
        }


        [MapToApiVersion("1.0")]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PagedResult<HolidayOverviewDto>>> GetHolidayByIdAsync(Guid id)
        {
            var response = await _mediator.Send(new GetHolidayByIdQuery(id));

            return response is not null? Ok(response) : NotFound();
        }


        [MapToApiVersion("1.0")]
        [HttpGet("snapshot/{id:guid}")]
        public async Task<ActionResult<PagedResult<HolidayToUpdateDto>>> GetHolidaySnapshotAsync(Guid id)
        {
            var response = await _mediator.Send(new GetHolidayToUpdateQuery(id));

            return response is not null ? Ok(response) : NotFound();
        }



        [MapToApiVersion("1.0")]
        [HttpPost]
        public async Task<ActionResult<Result>> CreateHolidayAsync([FromBody] HolidayCreateRequestDto request)
        {
            if ((request.StartDate is null || request.EndDate is null) && request.Day is null)
            {
                return BadRequest("Need a day or ange of date");
            }

            if (request.StartDate is not null && request.EndDate is null ||
                request.StartDate is null && request.EndDate is not null)
            {
                return BadRequest("Need a valid date range");
            }

            var command = _mapper.Map<CreateHolidayCommand>(request);

            var response = await _mediator.Send(command);

            return response.IsSuccess? Created() : BadRequest(response);
        }



        [MapToApiVersion("1.0")]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Result>> UpdateHolidayAsync(Guid id, [FromBody] HolidayUpdateRequestDto request)
        {
            var command = _mapper.Map<UpdateHolidayCommand>(request);

            command.HolidayId = id;

            var response = await _mediator.Send(command);

            return response.IsSuccess? NoContent() : BadRequest(response);
        }



        [MapToApiVersion("1.0")]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<Result>> DeleteHolidayAsync(Guid id)
        {
            var response = await _mediator.Send(new DeleteHolidayCommand
            {
                HolidayId = id
            });

            return response.IsSuccess? Ok() : BadRequest(response);
        }
    }
}
