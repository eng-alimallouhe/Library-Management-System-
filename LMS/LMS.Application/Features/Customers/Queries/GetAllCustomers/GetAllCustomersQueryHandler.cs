using AutoMapper;
using LMS.Application.Abstractions.Customers;
using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.Customers;
using MediatR;

namespace LMS.Application.Features.Customers.Queries.GetAllCustomers
{
    public class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, PagedResult<CustomersOverViewDto>>
    {
        private readonly ICustomerHelper _customerHelper;
        private readonly IMapper _mapper;


        public GetAllCustomersQueryHandler(
            ICustomerHelper customerHelper,
            IMapper mapper)
        {
            _customerHelper = customerHelper;
            _mapper = mapper;
        }


        public async Task<PagedResult<CustomersOverViewDto>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
        {
            var response = await _customerHelper.GetAllCustomersAsync(request.Filter);

            var customers = _mapper.Map<ICollection<CustomersOverViewDto>>(response.items);

            return new PagedResult<CustomersOverViewDto>
            {
                Items = customers,
                CurrentPage = request.Filter.PageNumber!.Value,
                PageSize = request.Filter.PageSize!.Value,
                TotalCount = response.count
            };
        }
    }
}