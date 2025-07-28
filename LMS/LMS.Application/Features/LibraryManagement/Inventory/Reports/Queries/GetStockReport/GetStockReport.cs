using LMS.Application.Filters.Inventory;
using LMS.Application.Filters.Reports;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Inventory.Reports.Queries.GetStockReport
{
    public record GetStockReportQuery(
        StockReportFilter Filter,
        ProductFilter ProductFilter) : IRequest<byte[]>;
}
