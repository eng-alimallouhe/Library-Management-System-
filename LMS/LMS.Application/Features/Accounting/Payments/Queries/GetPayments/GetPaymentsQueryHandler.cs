using AutoMapper;
using LMS.Application.Abstractions.Accounting;
using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.Financial;
using MediatR;

namespace LMS.Application.Features.Accounting.Payments.Queries.GetPayments
{
    public class GetPaymentsQueryHandler : IRequestHandler<GetPaymentsQuery, PagedResult<PaymentsDetailsDto>>
    {
        private readonly IFinancialHelper _financialHelper;
        private readonly IMapper _mapper;

        public GetPaymentsQueryHandler(
            IFinancialHelper financialHelper,
            IMapper mapper)
        {
            _financialHelper = financialHelper;
            _mapper = mapper;
        }

        public async Task<PagedResult<PaymentsDetailsDto>> Handle(GetPaymentsQuery request, CancellationToken cancellationToken)
        {
            var response = await _financialHelper.GetFilteredPaymentsAsync(request.Filter);

            var items = _mapper.Map<ICollection<PaymentsDetailsDto>>(response);

            return new PagedResult<PaymentsDetailsDto>
            {
                Items = items,
                TotalCount = response.count,
                CurrentPage = request.Filter.PageNumber!.Value,
                PageSize = request.Filter.PageSize!.Value
            };
        }
    }
}
