using AutoMapper;
using LMS.Application.Abstractions.Accounting;
using LMS.Application.Abstractions.Reports;
using LMS.Application.DTOs.Reports;
using LMS.Application.Filters.Finacial;
using MediatR;

namespace LMS.Application.Features.Accounting.Reports.Queries.GetFinancialReport
{
    public class GetFinancialReportQueryHandler : IRequestHandler<GetFinancialReportQuery, byte[]>
    {
        private readonly IReportsGeneratorHelper _reportsGenerator;
        private readonly IFinancialHelper _financialHelper;
        private readonly IMapper _mapper;

        public GetFinancialReportQueryHandler(
            IReportsGeneratorHelper reportsGenerator,
            IFinancialHelper financialHelper,
            IMapper mapper)
        {
            _reportsGenerator = reportsGenerator;
            _financialHelper = financialHelper;
            _mapper = mapper;
        }

        public async Task<byte[]> Handle(GetFinancialReportQuery request, CancellationToken cancellationToken)
        {
            (var payments, var paymentsCount) = await _financialHelper.GetFilteredPaymentsAsync(request.PaymentFilter);
            
            (var revenues, var revenuesCount) = await _financialHelper.GetFilteredRevenueAsync(request.RevenueFilter);

            var mappedPayments = _mapper.Map<ICollection<PaymentReportDto>>(payments);
            var mappedRevenues = _mapper.Map<ICollection<RevenueReportDto>>(revenues);

            return _reportsGenerator.GenerateFinancialReport(mappedRevenues, mappedPayments, request.Filter);
        }
    }
}
