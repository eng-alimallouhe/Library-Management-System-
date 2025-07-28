using LMS.Application.DTOs.Admin.Dashboard;
using LMS.Domain.LibraryManagement.Models.Products;

namespace LMS.Application.Abstractions.Services.Admin
{
    public interface IDashboardHelper 
    {
        Task<UsersCountDto> GetUsersCountAsync(DateTime from);
        Task<int> GetNewCustomersCounterAsync(DateTime from);
        Task<(int ordersCount, double ordersPercentage)> GetOrdersCountQueryAsync(DateTime from);
        Task<int> GetNewBooksCounter(DateTime from);
        Task<(int booksCount, double bookChangePercentage)> GetBooksCountAsync(DateTime from);

        Task<ICollection<Product>> GetLowStockQuantityAsync(int maxQuantity);

        Task<ICollection<MonthlyOrdersDto>> GetMonthlyOrdersAsync(DateTime from, DateTime to);

        Task<ICollection<MonthlySalesDto>> GetMonthlySalesAsync(DateTime from, DateTime to);

        Task<ICollection<TopSellingProductDto>> GetTopSellingProductsAsync(int topCoutn, int language);
    }
}