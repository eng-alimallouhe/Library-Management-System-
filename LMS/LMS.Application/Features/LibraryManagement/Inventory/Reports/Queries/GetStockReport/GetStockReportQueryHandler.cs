using AutoMapper;
using LMS.Application.Abstractions.LibraryManagement;
using LMS.Application.Abstractions.Reports;
using LMS.Application.DTOs.Stock;
using LMS.Application.Filters.Finacial;
using LMS.Application.Filters.Inventory;
using LMS.Domain.Identity.Enums;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Inventory.Reports.Queries.GetStockReport
{
    public class GetStockReportQueryHandler : IRequestHandler<GetStockReportQuery, byte[]>
    {
        private readonly IReportsGeneratorHelper _reportsGenerator;
        private readonly IInventoryHelper _inventoryHelper;
        private readonly IMapper _mapper;

        public GetStockReportQueryHandler(
            IReportsGeneratorHelper reportsGenerator,
            IInventoryHelper inventoryHelper,
            IMapper mapper)
        {
            _reportsGenerator = reportsGenerator;
            _inventoryHelper = inventoryHelper;
            _mapper = mapper;
        }

        public async Task<byte[]> Handle(GetStockReportQuery request, CancellationToken cancellationToken)
        {
            var products = await _inventoryHelper.GetInventorySnapshot(request.ProductFilter);

            var mappedProducts = _mapper.Map<ICollection<StockSnapshotDto>>(products.items, opts =>
        opts.Items["lang"] = request.ProductFilter.Language
                );

            return _reportsGenerator.GenerateStockReportAsync(mappedProducts, request.Filter);
        }
    }
}
