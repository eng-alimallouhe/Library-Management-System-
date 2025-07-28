using LMS.API.DTOs.QueriesParameter;
using LMS.Application.DTOs.Admin.Dashboard;
using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.Financial;
using LMS.Application.Features.Accounting.Revenues.Queries.GetRevenues;
using LMS.Application.Filters.Finacial;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers.Accounting
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/accounting/[controller]")]
    [ApiController]
    public class RevenuesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RevenuesController(
            IMediator mediator)
        {
            _mediator = mediator;
        }


        [MapToApiVersion("1.0")]
        [HttpGet]
        public async Task<ActionResult<PagedResult<RevenueDetailsDto>>> GetAllRevenues([FromQuery] RevenueFilter filter)
        {
            var command = new GetRevenuesQuery(filter);

            var response = await _mediator.Send(command);

            return Ok(response);
        }
    }
}
