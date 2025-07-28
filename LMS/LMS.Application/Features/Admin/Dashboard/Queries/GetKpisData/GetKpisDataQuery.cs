using LMS.Application.DTOs.Admin.Dashboard;
using MediatR;

namespace LMS.Application.Features.Admin.Dashboard.Queries.GetKpisData
{
    public record GetKpisDataQuery(
        DateTime From) : IRequest<DashboardKpiDto>; 
}
