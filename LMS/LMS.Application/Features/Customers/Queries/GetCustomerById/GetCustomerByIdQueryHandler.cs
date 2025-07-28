using AutoMapper;
using LMS.Application.Abstractions.Customers;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.DTOs.Customers;
using LMS.Domain.Customers.Models;
using MediatR;

namespace LMS.Application.Features.Customers.Queries.GetCustomerById
{
    public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDetailsDto?>
    {
        private readonly ISoftDeletableRepository<Customer> _customerRepo;
        private readonly ICustomerHelper _customerHelper;
        private readonly IMapper _mapper;


        public GetCustomerByIdQueryHandler(
            ISoftDeletableRepository<Customer> customerRepo,
            ICustomerHelper customerHelper,
            IMapper mapper)
        {
            _customerRepo = customerRepo;
            _customerHelper = customerHelper;
            _mapper = mapper;
        }


        public async Task<CustomerDetailsDto?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            var customer = await _customerHelper.BuildCustomerResponseAsync(request.userId);

            if (customer is null)
            {
                return null;
            }

            var result = _mapper.Map<CustomerDetailsDto>(customer, opt =>
                opt.Items["lang"] = request.Language);

            return result;
        }
    }
}
