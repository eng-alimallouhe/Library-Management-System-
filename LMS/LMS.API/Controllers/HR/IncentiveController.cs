using System.ComponentModel.DataAnnotations;
using AutoMapper;
using LMS.API.DTOs.HR;
using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.HR;
using LMS.Application.Features.HR.Incentives.Commands.AproveIncentive;
using LMS.Application.Features.HR.Incentives.Commands.CreateIncentive;
using LMS.Application.Features.HR.Incentives.Commands.DeleteIncentive;
using LMS.Application.Features.HR.Incentives.Commands.UpdateIncentive;
using LMS.Application.Features.HR.Incentives.Queries.GetAllIncentives;
using LMS.Application.Features.HR.Incentives.Queries.GetIncentiveById;
using LMS.Application.Filters.HR.EmployeeIdentity;
using LMS.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers.HR
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/hr/[controller]")]
    [ApiController]
    public class IncentiveController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        
        public IncentiveController(
            IMediator mediator,
            IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }


        [MapToApiVersion("1.0")]
        [HttpGet]
        public async Task<ActionResult<PagedResult<IncentiveOverviewDto>>> GetAllIncentivesAsync([FromQuery] IncentiveFilter filter)
        {
            var response = await _mediator.Send(new GetAllIncentivesQuery(filter));

            return Ok(response);
        }


        [MapToApiVersion("1.0")]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<IncentiveDetailsDto>> GetIncentiveByIdAsync(Guid id)
        {
            var response = await _mediator.Send(new GetIncentiveByIdQuery(id));

            return response != null? Ok(response) : NotFound();
        }


        [MapToApiVersion("1.0")]
        [HttpPost]
        public async Task<ActionResult<Result>> CreateIncentiveAsync([FromForm] IncentiveCreateRequestDto request)
        {
            var command = _mapper.Map<CreateIncentiveCommand>(request);

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
        public async Task<ActionResult<Result>> UpdateIncentive(Guid id, [FromForm] IncentiveUpdateRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = _mapper.Map<UpdateIncentiveCommand>(request);
            
            command.IncentiveId = id;

            if (request.Decision is not null)
            {
                using var stream = request.Decision.OpenReadStream();
                var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                var fileByByte = memoryStream.ToArray();

                command.DesicionFile = fileByByte;
            }
            else
            {
                command.DesicionFile = null;
            }

            var response = await _mediator.Send(command);

            return response.IsSuccess? Ok(response) : BadRequest(response);
        }



        [MapToApiVersion("1.0")]
        [HttpPut("approve/{id:guid}")]
        public async Task<ActionResult<Result>> ApproveIncentive([Required] Guid id,[FromQuery] bool isAproved)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _mediator.Send(new AproveIncentiveCommand
            {
                IncentiveId = id,
                IsApproved = isAproved
            });

            return response.IsSuccess ? NoContent() : BadRequest(response);
        }


        [MapToApiVersion("1.0")]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<Result>> DeletePenalty(Guid id)
        {
            var response = await _mediator.Send(new DeleteIncentiveCommand{
                IncentiveId = id
            });

            return response.IsSuccess? Ok(response) : BadRequest(response);
        }
    }
}