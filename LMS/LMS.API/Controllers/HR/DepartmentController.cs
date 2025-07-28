using System.ComponentModel.DataAnnotations;
using AutoMapper;
using LMS.API.DTOs.HR.Department;
using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.HR.Departments;
using LMS.Application.Features.HR.Departments.Command.CreateDepartment;
using LMS.Application.Features.HR.Departments.Command.DeleteDepartment;
using LMS.Application.Features.HR.Departments.Command.UpdateDepartment;
using LMS.Application.Features.HR.Departments.Queries.GetAllDepartments;
using LMS.Application.Features.HR.Departments.Queries.GetAvaliableDepartments;
using LMS.Application.Features.HR.Departments.Queries.GetDepartmentById;
using LMS.Application.Features.HR.Departments.Queries.GetDepartmentForUpdate;
using LMS.Application.Features.HR.Departments.Queries.SearchDepartmentsByName;
using LMS.Application.Filters.HR;
using LMS.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers.HR
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/hr/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public DepartmentController(
            IMapper mapper,
            IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        [MapToApiVersion("1.0")]
        [HttpGet]
        public async Task<ActionResult<PagedResult<DepartmentOverviewDto>>> GetAll([Required][FromQuery] DepartmentFilter filter)
        {
            var response = await _mediator.Send(new GetAllDepartmentsQuery(filter));

            return Ok(response);
        }

        [MapToApiVersion("1.0")]
        [HttpGet("max-count")]
        public async Task<ActionResult<int>> GetMaxEmployeeNumber()
        {
            var response = await _mediator.Send(new GetAllDepartmentsQuery(new DepartmentFilter
            {
                PageNumber = 1,
                PageSize = 1000,
                Language = 0
            }));

            var dep = response.Items.Max(e => e.EmployeesCount);

            return Ok(dep);
        } 


        [MapToApiVersion("1.0")]
        [HttpGet("{departmentName}")]
        public async Task<ActionResult<ICollection<DepartmentLookupDto>>> SearchDepartmentByName(string? departmentName)
        {
            var response = await _mediator.Send(new SearchDepartmentsByNameQuery(departmentName));

            return Ok(response);
        }

        [MapToApiVersion("1.0")]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<DepartmentDetailsDTO>> GetDepartmentById(Guid id)
        {
            var response = await _mediator.Send(new GetDepartmentByIdQuery(id));

            return Ok(response);
        }


        [MapToApiVersion("1.0")]
        [HttpGet("snapshot/{id:guid}")]
        public async Task<ActionResult<DepartmentUpdateDto>> GetDepartmentSnapshot(Guid id)
        {
            var response = await _mediator.Send(new GetDepartmentForUpdateQuery(id));

            return Ok(response);
        }


        [MapToApiVersion("1.0")]
        [HttpGet("avaliable-departments")]
        public async Task<ICollection<DepartmentLookupDto>> GetAvaliableDepartments([FromQuery] Guid? employeeId)
        {
            var response = await _mediator.Send(new GetAvaliableDepartmentsQuery(employeeId));

            return response;
        }



        [MapToApiVersion("1.0")]
        [HttpPost]
        public async Task<ActionResult<Result>> AddDepartment(DepartmentRequestDto request)
        {
            var command = _mapper.Map<CreateDepartmentCommand>(request);

            var response = await _mediator.Send(command);

            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }



        [MapToApiVersion("1.0")]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Result>> UpdateDepartment([FromBody]DepartmentRequestDto request, Guid id)
        {
            var command = _mapper.Map<UpdateDepartmentCommand>(request);

            command.DepartmentId = id;

            var response = await _mediator.Send(command);

            return response.IsSuccess ? Ok(response) : NotFound(response);
        }


        [MapToApiVersion("1.0")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Result>> DeleteDepartment(Guid id)
        {
            var response = await _mediator.Send(new DeleteDepartmentCommand()
            {
                DepartmentId = id
            });

            return response.IsSuccess ? Ok(response) : NotFound();
        }
    }
}
