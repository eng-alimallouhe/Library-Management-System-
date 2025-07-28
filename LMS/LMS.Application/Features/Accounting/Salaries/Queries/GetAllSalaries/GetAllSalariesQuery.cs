using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.HR.Employees;
using LMS.Application.Filters.HR;
using MediatR;

namespace LMS.Application.Features.Accounting.Salaries.Queries.GetAllSalaries
{
    public record GetAllSalariesQuery(SalaryFilter Filter) : IRequest<PagedResult<SalaryDetailsDto>>;
}
