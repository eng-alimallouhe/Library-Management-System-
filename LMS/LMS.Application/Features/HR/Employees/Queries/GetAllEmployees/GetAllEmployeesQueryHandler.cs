using AutoMapper;
using LMS.Application.Abstractions.HR;
using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.HR.Employees;
using MediatR;
// لا نحتاج لـ IUnitOfWork أو IAppLogger هنا، لأن هذا استعلام قراءة فقط

namespace LMS.Application.Features.HR.Employees.Queries.GetAllEmployees
{
    // تصحيح اسم الكلاس: يجب أن يكون GetAllEmployeesQueryHandler
    public class GetAllEmployeesQueryHandler : IRequestHandler<GetAllEmployeesQuery, PagedResult<EmployeeOverviewDto>>
    {
        private readonly IEmployeeHelper _employeeHelper;
        private readonly IMapper _mapper;

        public GetAllEmployeesQueryHandler(
            IEmployeeHelper employeeHelper,
            IMapper mapper)
        {
            _employeeHelper = employeeHelper;
            _mapper = mapper;
        }

        public async Task<PagedResult<EmployeeOverviewDto>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
        {
            // 1. استدعاء الدالة المساعدة لجلب البيانات المصفاة والمقسمة
            // (التي بدورها تستخدم الـ Repository والـ Specification)
            var (items, count) = await _employeeHelper.GetEmployeesPageAsync(request.Filter);

            // 2. تعيين الكيانات إلى DTOs باستخدام AutoMapper
            var employees = _mapper.Map<ICollection<EmployeeOverviewDto>>(items);

            // 3. بناء PagedResult وإرجاعها
            return new PagedResult<EmployeeOverviewDto>
            {
                Items = employees,
                TotalCount = count, // استخدم المتغير 'count' الذي تم إرجاعه من الدالة المساعدة
                PageSize = request.Filter.PageSize ?? 0, // استخدم .Value بأمان أو قيمة افتراضية إذا كان null
                CurrentPage = request.Filter.PageNumber ?? 0, // استخدم .Value بأمان أو قيمة افتراضية إذا كان null
            };
        }
    }
}