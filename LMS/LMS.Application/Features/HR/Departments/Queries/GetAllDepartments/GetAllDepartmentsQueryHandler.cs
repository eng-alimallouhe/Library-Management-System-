using AutoMapper;
using LMS.Application.Abstractions.HR;
using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.HR.Departments;
using MediatR;

namespace LMS.Application.Features.HR.Departments.Queries.GetAllDepartments
{
    public class GetAllDepartmentsQueryHandler : IRequestHandler<GetAllDepartmentsQuery, PagedResult<DepartmentOverviewDto>>
    {
        private readonly IDepartmentHelper _departmentHelper;
        private readonly IMapper _mapper;

        public GetAllDepartmentsQueryHandler(
            IDepartmentHelper departmentHelper,
            IMapper mapper)
        {
            _departmentHelper = departmentHelper;
            _mapper = mapper;
        }

        public async Task<PagedResult<DepartmentOverviewDto>> Handle(GetAllDepartmentsQuery request, CancellationToken cancellationToken)
        {
            var response = await _departmentHelper.GetDepartmentsAsync(request.Filter);


            var departments =  _mapper.Map<ICollection<DepartmentOverviewDto>>(response.items);


            return new PagedResult<DepartmentOverviewDto>
            {
                Items = departments,
                PageSize = request.Filter.PageSize!.Value,
                CurrentPage = request.Filter.PageNumber!.Value,
                TotalCount = response.count
            };
        }
    }
}
