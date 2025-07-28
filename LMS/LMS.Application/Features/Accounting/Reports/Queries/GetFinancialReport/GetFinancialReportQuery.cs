using LMS.Application.Filters.Finacial;
using LMS.Application.Filters.Reports;
using MediatR;

namespace LMS.Application.Features.Accounting.Reports.Queries.GetFinancialReport
{
    public record GetFinancialReportQuery(
        FinancialReportFilter Filter,
        PaymentFilter PaymentFilter,
        RevenueFilter RevenueFilter) : IRequest<byte[]>;
}
