using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.Financial;
using LMS.Application.Filters.Finacial;
using MediatR;

namespace LMS.Application.Features.Accounting.Payments.Queries.GetPayments
{
    public record GetPaymentsQuery(
        PaymentFilter Filter) : IRequest<PagedResult<PaymentsDetailsDto>>;
}
