using AutoMapper;
using LMS.Application.Abstractions.Accounting;
using LMS.Application.Abstractions.Reports;
using LMS.Application.DTOs.Reports;
using LMS.Application.Filters.Finacial;
using MediatR;

namespace LMS.Application.Features.Accounting.Reports.Queries.GetPaymentsReport
{
    public class GetPaymentsReportQueryHanlder : IRequestHandler<GetPaymentsReportQuery, byte[]>
    {
        private readonly IReportsGeneratorHelper _reportsGenerator;
        private readonly IFinancialHelper _financialHelper;
        private readonly IMapper _mapper;

        public GetPaymentsReportQueryHanlder(
            IReportsGeneratorHelper reportsGenerator,
            IFinancialHelper financialHelper,
            IMapper mapper)
        {
            _reportsGenerator = reportsGenerator;
            _financialHelper = financialHelper;
            _mapper = mapper;
        }

        public async Task<byte[]> Handle(GetPaymentsReportQuery request, CancellationToken cancellationToken)
        {
            (var payments, var paymentsCount) = await _financialHelper.GetFilteredPaymentsAsync(request.PaymentFilter);

            var mappedPayments = _mapper.Map<ICollection<PaymentReportDto>>(payments);

            return _reportsGenerator.GeneratePaymentReport(mappedPayments, request.Filter);
        }
    }
}
