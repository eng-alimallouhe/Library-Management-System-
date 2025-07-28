using LMS.Application.DTOs.Reports;
using LMS.Application.Filters.Finacial;
using MediatR;

namespace LMS.Application.Features.Accounting.Reports.Queries.GetPaymentsReport
{
    public record GetPaymentsReportQuery(
        PaymentReportFilter Filter,
        PaymentFilter PaymentFilter) : IRequest<byte[]>;
}
