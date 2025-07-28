using AutoMapper;
using LMS.Application.Abstractions.HR;
using LMS.Application.DTOs.HR.Departments;
using MediatR;

namespace LMS.Application.Features.HR.Departments.Queries.GetAvaliableDepartments
{
    public class GetAvaliableDepartmentsQueryHandler : IRequestHandler<GetAvaliableDepartmentsQuery, ICollection<DepartmentLookupDto>>
    {
        private readonly IDepartmentHelper _departmentHelper;
        private readonly IMapper _mapper;

        public GetAvaliableDepartmentsQueryHandler(
            IDepartmentHelper departmentHelper, 
            IMapper mapper)
        {
            _departmentHelper = departmentHelper;
            _mapper = mapper;
        }

        public async Task<ICollection<DepartmentLookupDto>> Handle(GetAvaliableDepartmentsQuery request, CancellationToken cancellationToken)
        {
            var response = await _departmentHelper.GetAvaliableDepartmentsAsync(request.EmployeeId);

            return _mapper.Map<ICollection<DepartmentLookupDto>>(response);
        }
    }
}