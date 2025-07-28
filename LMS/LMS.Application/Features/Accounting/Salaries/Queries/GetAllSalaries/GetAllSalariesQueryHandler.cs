using AutoMapper;
using LMS.Application.Abstractions.Accounting;
using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.HR.Employees;
using MediatR;

namespace LMS.Application.Features.Accounting.Salaries.Queries.GetAllSalaries
{
    public class GetAllSalariesQueryHandler : IRequestHandler<GetAllSalariesQuery, PagedResult<SalaryDetailsDto>>
    {
        private readonly IFinancialHelper _helper;
        private readonly IMapper _mapper;

        public GetAllSalariesQueryHandler(IFinancialHelper helper, IMapper mapper)
        {
            _helper = helper;
            _mapper = mapper;
        }

        public async Task<PagedResult<SalaryDetailsDto>> Handle(GetAllSalariesQuery request, CancellationToken cancellationToken)
        {
            var (items, count) = await _helper.GetAllSalariesAsync(request.Filter);
            
            var mapped = _mapper.Map<ICollection<SalaryDetailsDto>>(items);

            return new PagedResult<SalaryDetailsDto>
            {
                Items = mapped,
                TotalCount = count,
                PageSize = request.Filter.PageSize!.Value,
                CurrentPage = request.Filter.PageNumber!.Value
            };
        }
    }
}
