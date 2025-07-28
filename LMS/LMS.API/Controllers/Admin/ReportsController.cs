using System.ComponentModel.DataAnnotations;
using LMS.Application.DTOs.Reports;
using LMS.Application.Features.Accounting.Reports.Queries.GetFinancialReport;
using LMS.Application.Features.Accounting.Reports.Queries.GetPaymentsReport;
using LMS.Application.Features.Accounting.Reports.Queries.GetRevenuesReport;
using LMS.Application.Features.LibraryManagement.Inventory.Reports.Queries.GetDeadStockReport;
using LMS.Application.Features.LibraryManagement.Inventory.Reports.Queries.GetStockReport;
using LMS.Application.Filters.Finacial;
using LMS.Application.Filters.Inventory;
using LMS.Application.Filters.Reports;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        IMediator _mediator;
        
        public ReportsController(
            IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet("stock")]
        public async Task<ActionResult<byte[]>> GetStockReport([FromQuery][Required] StockReportFilter reportFilter, [FromQuery][Required] ProductFilter productFilter)
        {
            var reportData = await _mediator.Send(new GetStockReportQuery(reportFilter, productFilter));
            

            return File(
                reportData,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"inventory_report_{new Random().Next(11111, 99999)}.xlsx");
        }


        [HttpGet("dead-stock")]
        public async Task<ActionResult<byte[]>> GetDeadStockReport([FromQuery][Required] DeadStockReportFilter reportFilter, [FromQuery][Required] DeadStockFilter stockFilter)
        {
            var reportData = await _mediator.Send(new GetDeadStockReportQuery(reportFilter, stockFilter));

            return File(
                reportData,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"dead_stack_report_{new Random().Next(11111, 9999)}.xlsx");
        }


        [HttpGet("financial")]
        public async Task<ActionResult<byte[]>> GetFinancialReport([FromQuery]FinancialReportFilter reportFilter, [FromQuery] PaymentFilter paymentFilter, [FromQuery]RevenueFilter revenueFilter)
        {
            var revReportData = await _mediator.Send(new GetFinancialReportQuery(reportFilter, paymentFilter, revenueFilter));
            
            return File(
                revReportData,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"financial_report_{new Random().Next(11111, 9999)}.xlsx"
                );
        }


        [HttpGet("revenues")]
        public async Task<ActionResult<byte[]>> GetRevenuesReport([FromQuery] RevenueReportFilter reportFilter, [FromQuery] RevenueFilter revenueFilter)
        {
            var revReportData = await _mediator.Send(new GetRevenuesReportQuery(reportFilter, revenueFilter));

            return File(
                revReportData,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"revenues_report_{new Random().Next(11111, 9999)}.xlsx"
                );
        }



        [HttpGet("payments")]
        public async Task<ActionResult<byte[]>> GetPaymentsReport([FromQuery] PaymentReportFilter reportFilter, [FromQuery] PaymentFilter paymentFilter)
        {
            var revReportData = await _mediator.Send(new GetPaymentsReportQuery(reportFilter, paymentFilter));

            return File(
                revReportData,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"financial_report_{new Random().Next(11111, 9999)}.xlsx"
                );
        }
    }
}
