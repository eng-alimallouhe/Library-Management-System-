using LMS.Application.Filters.Inventory;
using LMS.Application.Filters.Reports;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Inventory.Reports.Queries.GetDeadStockReport
{
    public record GetDeadStockReportQuery(
        DeadStockReportFilter Filter,
        DeadStockFilter DeadStockFilter) : IRequest<byte[]>;
}
