using AutoMapper;
using LMS.API.DTOs.HR;
using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.HR;
using LMS.Application.Features.HR.Penalties.Commands.AddPenalty;
using LMS.Application.Features.HR.Penalties.Commands.ApprovePenalty;
using LMS.Application.Features.HR.Penalties.Commands.DeletePenalty;
using LMS.Application.Features.HR.Penalties.Commands.UpdatePenalty;
using LMS.Application.Features.HR.Penalties.Queries.GetAllPenalties;
using LMS.Application.Features.HR.Penalties.Queries.GetPenaltyById;
using LMS.Application.Filters.HR.EmployeeIdentity;
using LMS.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers.HR
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/hr/[controller]")]
    [ApiController]
    public class PenaltyController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        
        public PenaltyController(
            IMediator mediator,
            IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }


        [MapToApiVersion("1.0")]
        [HttpGet]
        public async Task<ActionResult<PagedResult<PenaltyOverviewDto>>> GetAllPenalties([FromQuery] PenaltyFilter filter)
        {
            var response = await _mediator.Send(new GetAllPenaltiesQuery(filter));

            return Ok(response);
        }

        [MapToApiVersion("1.0")]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PenaltyDetailsDto>> GetPenaltyById(Guid id)
        {
            var response = await _mediator.Send(new GetPenaltyByIdQuery(id));

            return response != null? Ok(response) : NotFound();
        }


        [MapToApiVersion("1.0")]
        [HttpPost]
        public async Task<ActionResult<Result>> CreatePenalty([FromForm] PenaltyCreateRequestDto request)
        {
            var command = _mapper.Map<AddPenaltyCommand>(request);

            using var stream = request.DecisionFile.OpenReadStream();
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var fileByByte = memoryStream.ToArray();

            command.DecisionFile = fileByByte;

            var response = await _mediator.Send(command);

            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }


        [MapToApiVersion("1.0")]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Result>> UpdatePenalty(Guid id, [FromForm] PenaltyUpdateRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = _mapper.Map<UpdatePenaltyCommand>(request);
            command.PenaltyId = id;

            if (request.Decision is not null)
            {
                using var stream = request.Decision.OpenReadStream();
                var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                var fileByByte = memoryStream.ToArray();

                command.DesicionFile = fileByByte;
            }

            var response = await _mediator.Send(command);

            return response.IsSuccess? Ok(response) : BadRequest(response);
        }



        [MapToApiVersion("1.0")]
        [HttpPut("approve/{id:guid}")]
        public async Task<ActionResult<Result>> ApprovePenalty(Guid id, [FromQuery] bool isAproved)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _mediator.Send(new ApprovePenaltyCommand
            {
                PenaltyId = id,
                IsAproved = isAproved
            });

            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }


        [MapToApiVersion("1.0")]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<Result>> DeletePenalty(Guid id)
        {
            var response = await _mediator.Send(new DeletePenaltyCommand{
                PenaltyId= id
            });

            return response.IsSuccess? Ok(response) : BadRequest(response);
        }
    }
}