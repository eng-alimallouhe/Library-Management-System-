﻿using LMS.Application.Abstractions.Services.Admin;
using LMS.Application.DTOs.Admin.Dashboard;
using MediatR;

namespace LMS.Application.Features.Admin.Dashboard.Queries.GetMonthlyOrders
{
    public class MonthlyOrdersQueryHandler : IRequestHandler<MonthlyOrdersQuery, ICollection<MonthlyOrdersDto>>
    {
        private readonly IDashboardHelper _dashboradHelper;

        public MonthlyOrdersQueryHandler(
            IDashboardHelper dashboradHelper)
        {
            _dashboradHelper = dashboradHelper;
        }

        public async Task<ICollection<MonthlyOrdersDto>> Handle(MonthlyOrdersQuery request, CancellationToken cancellationToken)
        {
            return await _dashboradHelper.GetMonthlyOrdersAsync(request.From, request.To);
        }
    }
}
