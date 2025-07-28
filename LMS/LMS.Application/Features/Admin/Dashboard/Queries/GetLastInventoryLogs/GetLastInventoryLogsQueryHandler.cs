using AutoMapper;
using LMS.Application.Abstractions.LibraryManagement;
using LMS.Application.DTOs.Stock;
using MediatR;

namespace LMS.Application.Features.Admin.Dashboard.Queries.GetLastInventoryLogs
{
    public class GetLastInventoryLogsQueryHandler : IRequestHandler<GetLastInventoryLogsQuery, ICollection<InventoryLogOverviewDto>>
    {
        private readonly IInventoryHelper _inventoryHelper;
        private readonly IMapper _mapper;

        public GetLastInventoryLogsQueryHandler(
            IInventoryHelper inventoryHelper,
            IMapper mapper)
        { 
            _inventoryHelper = inventoryHelper;
            _mapper = mapper;
        }

        public async Task<ICollection<InventoryLogOverviewDto>> Handle(GetLastInventoryLogsQuery request, CancellationToken cancellationToken)
        {
            var response = await _inventoryHelper.GetLastInventoryLogsAsync(request.Language);

            return _mapper.Map<ICollection<InventoryLogOverviewDto>>(response.items);
        }
    }
}
