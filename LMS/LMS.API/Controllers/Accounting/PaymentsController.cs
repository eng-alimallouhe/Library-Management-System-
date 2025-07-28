using LMS.API.DTOs.QueriesParameter;
using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.Financial;
using LMS.Application.Features.Accounting.Payments.Queries.GetPayments;
using LMS.Application.Filters.Finacial;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers.Accounting
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/accounting/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentsController(
            IMediator mediator)
        {
            _mediator = mediator;
        }


        [MapToApiVersion("1.0")]
        [HttpGet]
        public async Task<ActionResult<PagedResult<PaymentsDetailsDto>>> GetAllExpenses([FromQuery] PaymentFilter filter)
        {
            var command = new GetPaymentsQuery(filter);

            var response = await _mediator.Send(command);

            return Ok(response);
        }
    }
}
