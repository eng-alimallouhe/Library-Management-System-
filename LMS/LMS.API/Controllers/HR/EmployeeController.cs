using System.ComponentModel.DataAnnotations;
using AutoMapper;
using LMS.API.DTOs.HR.Employee;
using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.HR.Employees;
using LMS.Application.Features.HR.Employees.Command.CreateEmployee;
using LMS.Application.Features.HR.Employees.Command.DeleteEmployee;
using LMS.Application.Features.HR.Employees.Command.TransferEmployee;
using LMS.Application.Features.HR.Employees.Command.UpdateEmployee;
using LMS.Application.Features.HR.Employees.Queries.GetAllEmployees;
using LMS.Application.Features.HR.Employees.Queries.GetEmployeeById;
using LMS.Application.Features.HR.Employees.Queries.GetEmployeeForUpdate;
using LMS.Application.Filters.HR;
using LMS.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers.HR
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/hr/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public EmployeeController(
            IMapper mapper,
            IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }



        [MapToApiVersion("1.0")]
        [HttpGet]
        public async Task<ActionResult<PagedResult<EmployeeOverviewDto>>> GetAll([Required][FromQuery] EmployeeFilter filter)
        {
            var response = await _mediator.Send(new GetAllEmployeesQuery(filter));

            return Ok(response);
        }


        [MapToApiVersion("1.0")]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EmployeeDetailsDto>> GetEmployeeById(Guid id, [FromQuery] int language)
        {
            var response = await _mediator.Send(new GetEmployeeByIdQuery(id));

            return response is null? NotFound() : Ok(response);
        }



        [MapToApiVersion("1.0")]
        [HttpGet("snapshot/{employeeId:guid}")]
        public async Task<ActionResult<EmployeeUpdateDto?>> GetEmployeeSnapshot(Guid employeeId)
        {
            var response = await _mediator.Send(new GetEmployeeForUpdateQuery(employeeId));

            return response is null ? NotFound() : Ok(response);
        }



        [MapToApiVersion("1.0")]
        [HttpPost]
        public async Task<ActionResult<Result<EmployeeCreatedDto>>> AddEmployee([FromForm] EmployeeCreateRequestDto request)
        {
            using var stream = request.AppointmentDecisionFile.OpenReadStream();
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream); 
            memoryStream.Position = 0;

            var fileByByte = memoryStream.ToArray();

            using var faceStream = request.FaceImage.OpenReadStream();
            var faceMemoryStream = new MemoryStream();
            await faceStream.CopyToAsync(faceMemoryStream);
            faceMemoryStream.Position = 0;

            var faceFileByByte = faceMemoryStream.ToArray();


            var command = _mapper.Map<CreateEmployeeCommand>(request);

            command.FaceImage = faceFileByByte;
            command.AppointmentDecision = fileByByte;

            var response = await _mediator.Send(command);

            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }



        [MapToApiVersion("1.0")]
        [HttpPut("transfer")]
        public async Task<ActionResult<Result>> TransferEmployee([FromForm]TransferEmployeeRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = _mapper.Map<TransferEmployeeCommand>(request);

            using var stream = request.AppointmentDesicionFile.OpenReadStream();
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var fileByByte = memoryStream.ToArray();

            command.AppointmentDecision = fileByByte;

            var response = await _mediator.Send(command);

            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        
        
        [MapToApiVersion("1.0")]
        [HttpPut("{employeeId:guid}")]
        public async Task<ActionResult<Result>> UpdateEmployee([FromForm]EmployeeUpdateRequestDto request, [FromRoute] Guid employeeId)
        {
            var command = _mapper.Map<UpdateEmployeeCommand>(request);

            command.EmployeeId = employeeId;

            if (request.EmployeeFaceImage != null)
            {
                using var faceStream = request.EmployeeFaceImage.OpenReadStream();
                var faceMemoryStream = new MemoryStream();
                await faceStream.CopyToAsync(faceMemoryStream);
                faceMemoryStream.Position = 0;

                var faceFileByByte = faceMemoryStream.ToArray();

                command.FaceImage = faceFileByByte;
            }

            var response = await _mediator.Send(command);
            
            return response.IsSuccess ? Ok(response) : NotFound(response);
        }



        [MapToApiVersion("1.0")]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<Result>> DeleteEmployee(Guid id)
        {
            var response = await _mediator.Send(new DeleteEmployeeCommand() 
            { 
                EmployeeId = id
            });

            return response.IsSuccess ? Ok(response) : NotFound(response);
        }

    }
}