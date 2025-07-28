using AutoMapper;
using LMS.Application.Abstractions.Accounting;
using LMS.Application.Abstractions.Reports;
using LMS.Application.DTOs.Stock;
using LMS.Application.Filters.Inventory;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Inventory.Reports.Queries.GetDeadStockReport
{
    public class GetDeadStockReportQueryHandler : IRequestHandler<GetDeadStockReportQuery, byte[]>
    {
        private readonly IReportsGeneratorHelper _reportsGenerator;
        private readonly IFinancialHelper _financialHelper;
        private readonly IMapper _mapper;

        public GetDeadStockReportQueryHandler(
            IReportsGeneratorHelper reportsGenerator,
            IFinancialHelper financialHelper,
            IMapper mapper)
        {
            _reportsGenerator = reportsGenerator;
            _financialHelper = financialHelper;
            _mapper = mapper;
        }


        public async Task<byte[]> Handle(GetDeadStockReportQuery request, CancellationToken cancellationToken)
        {
            var products = await _financialHelper.GetDeadStockAsync(request.DeadStockFilter);

            var mappedProducts = _mapper.Map<ICollection<DeadStockDto>>(products.items);

            return _reportsGenerator.GenerateDeadStockReport(mappedProducts, request.Filter);
        }
    }
}