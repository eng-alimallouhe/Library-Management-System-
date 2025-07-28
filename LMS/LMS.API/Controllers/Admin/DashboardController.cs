using System.ComponentModel.DataAnnotations;
using LMS.API.DTOs.QueriesParameter;
using LMS.Application.DTOs.Admin.Dashboard;
using LMS.Application.DTOs.Stock;
using LMS.Application.Features.Admin.Dashboard.Queries.GetKpisData;
using LMS.Application.Features.Admin.Dashboard.Queries.GetMonthlyOrders;
using LMS.Application.Features.Admin.Dashboard.Queries.GetMonthlySales;
using LMS.Application.Features.Admin.Dashboard.Queries.GetTopFiveSalesProducts;
using LMS.Application.Features.LibraryManagement.Inventory.Queries.GetProductsBelowThreshold;
using LMS.Application.Filters.Inventory;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers.Admin
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IMediator _mediator;
        
        
        public DashboardController(
            IMediator mediator)
        {
            _mediator = mediator;
        }

        [MapToApiVersion("1.0")]
        [HttpGet("kpi/{from:datetime}")]
        public async Task<ActionResult<DashboardKpiDto>> GetKpisData(DateTime from)
        {
            var response = await _mediator.Send(new GetKpisDataQuery(from));
            
            return response;
        }

        [MapToApiVersion("1.0")]
        [HttpGet("low-stock")]
        public async Task<ActionResult<ICollection<StockSnapshotDto>>> GetLowStock([FromQuery][Required] ProductFilter filter)
        {
            var command = new GetProductsBelowThresholdQuery(filter);

            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [MapToApiVersion("1.0")]
        [HttpGet("monthly-orders")]
        public async Task<ActionResult<ICollection<MonthlyOrdersDto>>> GetMonthlyOrders([FromQuery][Required] DateQueryParameter queryParameter)
        {
            if (queryParameter.From >= queryParameter.To)
            {
                return BadRequest();
            }

            var command = new MonthlyOrdersQuery(queryParameter.From, queryParameter.To);

            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [MapToApiVersion("1.0")]
        [HttpGet("monthly-sales")]
        public async Task<ActionResult<ICollection<MonthlySalesDto>>> GetMonthlySales([FromQuery][Required] DateQueryParameter queryParameter)
        {
            if (queryParameter.From >= queryParameter.To)
            {
                return BadRequest();
            }

            var command = new MonthlySalesQuery(queryParameter.From, queryParameter.To);

            var response = await _mediator.Send(command);

            return Ok(response);
        }


        [MapToApiVersion("1.0")]
        [HttpGet("top-sales-products")]
        public async Task<ActionResult<ICollection<TopSellingProductDto>>> GetTopSalesProducts([FromQuery][Required] int language)
        {
            var command = new TopFiveSalesProductsQuery(language);

            var reposne = await _mediator.Send(command);

            return Ok(reposne);
        }
    }
}