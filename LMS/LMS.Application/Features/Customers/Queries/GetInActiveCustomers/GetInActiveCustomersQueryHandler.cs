using AutoMapper;
using LMS.Application.Abstractions.Customers;
using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.Customers;
using MediatR;

namespace LMS.Application.Features.Admin.Customers.Queries.GetInActiveCustomers
{
    public class GetInActiveCustomersQueryHandler : IRequestHandler<GetInActiveCustomersQuery, PagedResult<InActiveCustomersDto>>
    {
        private readonly ICustomerHelper _customerHelper;
        private readonly IMapper _mapper;

        public GetInActiveCustomersQueryHandler(
            ICustomerHelper customerHelper,
            IMapper mapper)
        {
            _customerHelper = customerHelper;
            _mapper = mapper;   
        }

        public async Task<PagedResult<InActiveCustomersDto>> Handle(GetInActiveCustomersQuery request, CancellationToken cancellationToken)
        {
            var response = await _customerHelper.GetInActiveCustomersAsync(request.Filter);
            
            var inActiveCustomers = _mapper.Map<ICollection<InActiveCustomersDto>>(response.items);

            return new PagedResult<InActiveCustomersDto>
            {
                Items = inActiveCustomers,
                CurrentPage = request.Filter.PageNumber!.Value,
                PageSize = request.Filter.PageSize!.Value,
                TotalCount = response.count
            };
        }
    }
}