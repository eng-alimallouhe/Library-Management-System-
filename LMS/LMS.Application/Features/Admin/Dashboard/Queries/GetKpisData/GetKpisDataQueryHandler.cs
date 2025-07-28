using LMS.Application.Abstractions.Services.Admin;
using LMS.Application.DTOs.Admin.Dashboard;
using MediatR;

namespace LMS.Application.Features.Admin.Dashboard.Queries.GetKpisData
{
    public class GetKpisDataQueryHandler : IRequestHandler<GetKpisDataQuery, DashboardKpiDto>
    {
        private readonly IDashboardHelper _dashboardHelper;

        public GetKpisDataQueryHandler(IDashboardHelper dashboardHelper)
        {
            _dashboardHelper = dashboardHelper;
        }

        public async Task<DashboardKpiDto> Handle(GetKpisDataQuery request, CancellationToken cancellationToken)
        {
            var from = request.From;

            var userCount = await _dashboardHelper.GetUsersCountAsync(from);
            var booksCount = await _dashboardHelper.GetBooksCountAsync(from);
            var newBooksCount = await _dashboardHelper.GetNewBooksCounter(from);
            var orderCount = await _dashboardHelper.GetOrdersCountQueryAsync(from);

            return new DashboardKpiDto
            {
                UsersCount = userCount.UsersCount,
                CustomersCount = userCount.CustomersCount,
                EmployeesCount = userCount.EmployeesCount,
                UsersChangePercentage = userCount.UsersChangePercentage,
                CustomersChangePercentage = userCount.CustomersChangePercentage,
                EmployeesChangePercentage = userCount.EmployeesChangePercentage,
                NewCustomersCount = userCount.NewCustomersCount,
                BooksCount = booksCount.booksCount,
                BooksChangePercentage = booksCount.bookChangePercentage,
                NewBooksCount = newBooksCount,
                OrdersCount = orderCount.ordersCount,
                OrdersChangePercentage = orderCount.ordersPercentage
            };
        }
    }
}