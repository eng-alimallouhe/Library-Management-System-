using LMS.Application.DTOs.Stock;
using LMS.Application.Features.LibraryManagement.Movements.Queries.GetStockChangeLogs;
using LMS.Application.Filters.Inventory;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers.LibraryManagement
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class MovementController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MovementController(
            IMediator mediator)
        {
            _mediator = mediator;
        }


        [MapToApiVersion("1.0")]
        [HttpGet]
        public async Task<ActionResult<ICollection<DeadStockDto>>> GetAllMovementsAsunc([FromQuery] InventoryLogsFilter filter)
        {
            var response = await _mediator.Send(new GetStockChangeLogsQuery(filter));

            return Ok(response);
        }
    }
}
