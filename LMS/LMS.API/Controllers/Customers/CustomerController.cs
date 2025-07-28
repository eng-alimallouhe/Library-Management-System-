using System.ComponentModel.DataAnnotations;
using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.Customers;
using LMS.Application.Features.Admin.Customers.Queries.GetInActiveCustomers;
using LMS.Application.Features.Customers.Queries.GetAllCustomers;
using LMS.Application.Features.Customers.Queries.GetCustomerById;
using LMS.Application.Filters.Customers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers.Customers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CustomerController(
            IMediator mediator)
        {
            _mediator = mediator;
        }

        [MapToApiVersion("1.0")]
        [HttpGet]
        public async Task<ActionResult<PagedResult<CustomersOverViewDto>>> GetAllCustomers([FromQuery][Required] CustomersFilter filter)
        {
            var command = new GetAllCustomersQuery(filter);
        
            var response = await _mediator.Send(command);

            return Ok(response);
        }


        [MapToApiVersion("1.0")]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CustomerDetailsDto?>> GetCustomerById(Guid id,[FromQuery] int language)
        {
            var command = new GetCustomerByIdQuery(id, language);

            var response = await _mediator.Send(command);

            return response is null? NotFound() : Ok(response);
        }


        [MapToApiVersion("1.0")]
        [HttpGet("inactive-customers")]
        public async Task<ActionResult<PagedResult<InActiveCustomersDto>>> GetInactiveCustomers([FromQuery] CustomersFilter filter)
        {
            var command = new GetInActiveCustomersQuery(filter);

            var response = await _mediator.Send(command);

            return Ok(response);
        }
    }
}
