using AutoMapper;
using LMS.API.DTOs.HR;
using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.HR;
using LMS.Application.Features.HR.Leaves.Commands.AddLeave;
using LMS.Application.Features.HR.Leaves.Commands.DeleteLeave;
using LMS.Application.Features.HR.Leaves.Commands.UpdateLeave;
using LMS.Application.Features.HR.Leaves.Commands.UpdateLeaveStatus;
using LMS.Application.Features.HR.Leaves.Queries.GetAllLeaves;
using LMS.Application.Features.HR.Leaves.Queries.GetLeaveById;
using LMS.Application.Features.HR.Penalties.Commands.ApprovePenalty;
using LMS.Application.Features.HR.Penalties.Commands.DeletePenalty;
using LMS.Application.Features.HR.Penalties.Commands.UpdatePenalty;
using LMS.Application.Features.HR.Penalties.Queries.GetAllPenalties;
using LMS.Application.Features.HR.Penalties.Queries.GetPenaltyById;
using LMS.Application.Filters.HR.EmployeeIdentity;
using LMS.Common.Results;
using LMS.Domain.HR.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers.HR
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/hr/[controller]")]
    [ApiController]
    public class LeaveController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public LeaveController(
            IMapper mapper,
            IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        [MapToApiVersion("1.0")]
        [HttpGet]
        public async Task<ActionResult<PagedResult<LeaveOverviewDto>>> GetAllLeavesAsync([FromQuery] LeaveFilter filter)
        {
            var response = await _mediator.Send(new GetAllLeavesQuery(filter));

            return Ok(response);
        }


        [MapToApiVersion("1.0")]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<LeaveDetailsDto>> GetLeaveByIdAsync(Guid id)
        {
            var response = await _mediator.Send(new GetLeaveByIdQuery(id));

            return response != null ? Ok(response) : NotFound();
        }


        [MapToApiVersion("1.0")]
        [HttpPost]
        public async Task<ActionResult<Result>> AddLeaveAsunc(LeaveCreateRequestDto request)
        {
            var command = _mapper.Map<AddLeaveCommand>(request);

            var response = await _mediator.Send(command);

            return response.IsSuccess? Ok(response) : BadRequest(response);
        }

        [MapToApiVersion("1.0")]
        [HttpPut("approve/{id:guid}")]
        public async Task<ActionResult<Result>> ApproveLeaveAsync(Guid id, [FromQuery] bool isAproved)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _mediator.Send(new UpdateLeaveStatusCommand
            {
                LeaveId = id,
                NewStatus = isAproved? (int)LeaveStatus.Approved : (int)LeaveStatus.Rejected,
                UpdatedBy = Guid.NewGuid()
            });

            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }


        [MapToApiVersion("1.0")]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Result>> UpdateLeaveAsync(Guid id, [FromForm] LeaveUpdateRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = _mapper.Map<UpdateLeaveCommand>(request);
            command.LeaveId = id;


            var response = await _mediator.Send(command);

            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }


        [MapToApiVersion("1.0")]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<Result>> DeleteLeaveAsync(Guid id)
        {
            var response = await _mediator.Send(new DeleteLeaveCommand
            {
                LeaveId = id
            });

            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}
