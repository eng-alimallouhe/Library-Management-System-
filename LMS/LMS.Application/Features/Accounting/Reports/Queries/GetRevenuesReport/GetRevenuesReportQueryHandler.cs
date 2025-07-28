using AutoMapper;
using LMS.Application.Abstractions.Accounting;
using LMS.Application.Abstractions.Reports;
using LMS.Application.DTOs.Reports;
using LMS.Application.Filters.Finacial;
using LMS.Domain.Financial.Models;
using MediatR;

namespace LMS.Application.Features.Accounting.Reports.Queries.GetRevenuesReport
{
    public class GetRevenuesReportQueryHandler : IRequestHandler<GetRevenuesReportQuery, byte[]>
    {
        private readonly IReportsGeneratorHelper _reportsGenerator;
        private readonly IFinancialHelper _financialHelper;
        private readonly IMapper _mapper;

        public GetRevenuesReportQueryHandler(
            IReportsGeneratorHelper reportsGenerator,
            IFinancialHelper financialHelper,
            IMapper mapper)
        {
            _reportsGenerator = reportsGenerator;
            _financialHelper = financialHelper;
            _mapper = mapper;
        }

        public async Task<byte[]> Handle(GetRevenuesReportQuery request, CancellationToken cancellationToken)
        {
            (var revenues, var revenuesCount) = await _financialHelper.GetFilteredRevenueAsync(request.RevenueFilter);


            var mappedRevenues = _mapper.Map<ICollection<RevenueReportDto>>(revenues);

            return _reportsGenerator.GenerateRevenueReport(mappedRevenues, request.Filter);
        }
    }
}
