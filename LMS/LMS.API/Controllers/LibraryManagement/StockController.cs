using System.ComponentModel.DataAnnotations;
using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.Stock;
using LMS.Application.Features.LibraryManagement.Inventory.Queries.GetDeadProducts;
using LMS.Application.Features.LibraryManagement.Inventory.Queries.GetInventoryAudit;
using LMS.Application.Features.LibraryManagement.Movements.Queries.GetStockChangeLogs;
using LMS.Application.Filters.Inventory;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers.LibraryManagement
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/library-management/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StockController(
            IMediator mediator)
        {
            _mediator = mediator;
        }



        [MapToApiVersion("1.0")]
        [HttpGet("dead")]
        public async Task<ActionResult<ICollection<DeadStockDto>>> GetDeadStock([FromQuery][Required] DeadStockFilter filter)
        {
            var command = new GetDeadProductsQuery(filter);

            var response = await _mediator.Send(command);

            return Ok(response);
        }



        [MapToApiVersion("1.0")]
        [HttpGet("audit")]
        public async Task<ActionResult<PagedResult<StockSnapshotDto>>> GetStockSnapshot([FromQuery] ProductFilter filter)
        {
            var command = new GetInventoryAuditQuery(filter);

            var response = await _mediator.Send(command);

            return Ok(response);
        }
    }
}
