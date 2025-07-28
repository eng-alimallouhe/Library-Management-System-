using LMS.Application.Abstractions.Repositories;
using LMS.Application.Abstractions.Services.Admin;
using LMS.Application.DTOs.Admin.Dashboard;
using LMS.Domain.Customers.Models;
using LMS.Domain.Identity.Models;
using LMS.Domain.LibraryManagement.Models.Products;
using LMS.Domain.Orders.Enums;
using LMS.Domain.Orders.Models;
using LMS.Infrastructure.Specifications.Customers;
using LMS.Infrastructure.Specifications.Identity;
using LMS.Infrastructure.Specifications.LibraryManagement;
using LMS.Infrastructure.Specifications.Orders;
using LMS.Infrastructure.Specifications.Sales;

namespace LMS.Infrastructure.Helpers.Admin
{
    public class DashboardHelper : IDashboardHelper
    {
        private readonly ISoftDeletableRepository<Order> _baseOrderRepo;
        private readonly ISoftDeletableRepository<OrderItem> _orderItemRepo;
        private readonly ISoftDeletableRepository<Book> _bookRepo;
        private readonly ISoftDeletableRepository<Customer> _customerRepo;
        private readonly ISoftDeletableRepository<User> _userRepo;
        private readonly ISoftDeletableRepository<Product> _productRepo;

        public DashboardHelper(
            ISoftDeletableRepository<Order> baseOrderRepo,
            ISoftDeletableRepository<OrderItem> orderItemRepo,
            ISoftDeletableRepository<Book> bookRepo,
            ISoftDeletableRepository<Customer> customerRepo,
            ISoftDeletableRepository<User> userRepo,
            ISoftDeletableRepository<Product> productRepo)
        {
            _baseOrderRepo = baseOrderRepo;
            _orderItemRepo = orderItemRepo;
            _bookRepo = bookRepo;
            _customerRepo = customerRepo;
            _userRepo = userRepo;
            _productRepo = productRepo;
        }


        public async Task<(int booksCount, double bookChangePercentage)> GetBooksCountAsync(DateTime from)
        {
            var books = (await _bookRepo.GetAllAsync(new ActiveBooksSpecification(null))).items;

            DateTime currentPeriodStartDate = from;
            DateTime currentPeriodEndDate = DateTime.UtcNow;


            TimeSpan periodLength = currentPeriodEndDate - currentPeriodStartDate;

            DateTime previousPeriodEndDate = currentPeriodStartDate.AddDays(-1);
            DateTime previousPeriodStartDate = previousPeriodEndDate.Subtract(periodLength);


            var booksCurrentPeriodCount = books.Count(order => order.CreatedAt >= currentPeriodStartDate && order.CreatedAt <= currentPeriodEndDate);
            var booksPreviousPeriodCount = books.Count(order => order.CreatedAt >= previousPeriodStartDate && order.CreatedAt <= previousPeriodEndDate);

            double booksChangePercentage = CalculatePercentageChange(booksCurrentPeriodCount, booksPreviousPeriodCount);


            return (books.Count(), booksChangePercentage);
        }


        public async Task<int> GetNewBooksCounter(DateTime from)
        {
            return (await _bookRepo.GetAllAsync(new ActiveBooksSpecification(from))).count;
        }


        public async Task<int> GetNewCustomersCounterAsync(DateTime from)
        {
            return (await _customerRepo.GetAllAsync(new ActiveCustomersSpecification(from, false))).count;
        }
        
        public async Task<UsersCountDto> GetUsersCountAsync(DateTime from)
        {
            var response = await _userRepo.GetAllAsync(new ActiveUsersSpecification());
            var users = response.items;


            DateTime currentPeriodStartDate = from;
            DateTime currentPeriodEndDate = DateTime.UtcNow;


            TimeSpan periodLength = currentPeriodEndDate - currentPeriodStartDate;

            DateTime previousPeriodEndDate = currentPeriodStartDate.AddDays(-1);
            DateTime previousPeriodStartDate = previousPeriodEndDate.Subtract(periodLength);




            var usersCount = response.count;
            int usersCurrentPeriodCount = users.Count(user => user.CreatedAt >= currentPeriodStartDate && user.CreatedAt <= currentPeriodEndDate);
            int usersPreviousPeriodCount = users.Count(user => user.CreatedAt >= previousPeriodStartDate && user.CreatedAt <= previousPeriodEndDate);


            double usersChangePercentage = CalculatePercentageChange(usersCurrentPeriodCount, usersPreviousPeriodCount);



            var allEmployees = users.Where(user => user.Role != null && user.Role.RoleType.ToLower() == "employee").ToList();
            int employeesCount = allEmployees.Count();

            int employeesCurrentPeriodCount = allEmployees.Count(user => user.CreatedAt >= currentPeriodStartDate && user.CreatedAt <= currentPeriodEndDate);
            int employeesPreviousPeriodCount = allEmployees.Count(user => user.CreatedAt >= previousPeriodStartDate && user.CreatedAt <= previousPeriodEndDate);

            double employeesChangePercentage = CalculatePercentageChange(employeesCurrentPeriodCount, employeesPreviousPeriodCount);


            var allCustomers = users.Where(user => user.Role != null && user.Role.RoleType != null && user.Role.RoleType.ToLower() == "customer").ToList();
            int customersCount = allCustomers.Count();

            int customersCurrentPeriodCount = allCustomers.Count(user => user.CreatedAt >= currentPeriodStartDate && user.CreatedAt <= currentPeriodEndDate);
            int customersPreviousPeriodCount = allCustomers.Count(user => user.CreatedAt >= previousPeriodStartDate && user.CreatedAt <= previousPeriodEndDate);

            double customersChangePercentage = CalculatePercentageChange(customersCurrentPeriodCount, customersPreviousPeriodCount);

            int newCustomersCount = customersCurrentPeriodCount;




            return new UsersCountDto
            {
                UsersCount = usersCount,
                EmployeesCount = employeesCount,
                CustomersCount = customersCount,
                UsersChangePercentage = usersChangePercentage,
                EmployeesChangePercentage = employeesChangePercentage,
                CustomersChangePercentage = customersChangePercentage,
                NewCustomersCount = newCustomersCount,
            };
        }


        public async Task<(int ordersCount, double ordersPercentage)> GetOrdersCountQueryAsync(DateTime from)
        {
            var orders = await _baseOrderRepo.GetAllByExpressionAsync(
                o => o.Status != OrderStatus.Cancelled);
            
            DateTime currentPeriodStartDate = from;
            DateTime currentPeriodEndDate = DateTime.UtcNow;


            TimeSpan periodLength = currentPeriodEndDate - currentPeriodStartDate;

            DateTime previousPeriodEndDate = currentPeriodStartDate.AddDays(-1);
            DateTime previousPeriodStartDate = previousPeriodEndDate.Subtract(periodLength);


            var ordersCurrentPeriodCount = orders.Count(order => order.CreatedAt >= currentPeriodStartDate && order.CreatedAt <= currentPeriodEndDate);
            var ordersPreviousPeriodCount = orders.Count(order => order.CreatedAt >= previousPeriodStartDate && order.CreatedAt <= previousPeriodEndDate);

            double ordersChangePercentage = CalculatePercentageChange(ordersCurrentPeriodCount, ordersPreviousPeriodCount);

            return (orders.Count(), ordersChangePercentage);
        }

        public async Task<ICollection<Product>> GetLowStockQuantityAsync(int maxQuantity)
        {
            return (await _productRepo.GetAllAsync(new LowStockQuantitySpecification(1))).items;
        }

        public async Task<ICollection<MonthlyOrdersDto>> GetMonthlyOrdersAsync(DateTime from, DateTime to)
        {
            var response = await _baseOrderRepo.GetAllProjectedAsync(new MonthlyOrdersSpecification(from, to));
            return response;
        }

        public async Task<ICollection<TopSellingProductDto>> GetTopSellingProductsAsync(int topCoutn, int language)
        {
            var response = await _orderItemRepo.GetAllProjectedAsync(new TopSellingProductsSpecification(topCoutn, language));
            return response;
        }

        public async Task<ICollection<MonthlySalesDto>> GetMonthlySalesAsync(DateTime from, DateTime to)
        {
            var response = await _baseOrderRepo.GetAllProjectedAsync(new MonthlySalesFromBaseOrdersSpecification(from, to));
            return response;
        }


        private double CalculatePercentageChange(int currentValue, int previousValue)
        {
            if (previousValue == 0)
            {
                return currentValue > 0 ? 100.0 : 0.0;
            }
            double percentage = ((double)(currentValue - previousValue) / previousValue) * 100.0;
            return Math.Round(percentage, 2);
        }
    }
}
