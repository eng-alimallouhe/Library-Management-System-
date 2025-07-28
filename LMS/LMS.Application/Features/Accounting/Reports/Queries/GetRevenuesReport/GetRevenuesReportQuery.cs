using LMS.Application.Filters.Finacial;
using LMS.Application.Filters.Reports;
using MediatR;

namespace LMS.Application.Features.Accounting.Reports.Queries.GetRevenuesReport
{
    public record GetRevenuesReportQuery(
        RevenueReportFilter Filter,
        RevenueFilter RevenueFilter) : IRequest<byte[]>;
}
