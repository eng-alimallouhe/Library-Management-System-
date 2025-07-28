using LMS.Domain.Customers.Models;
using LMS.Domain.Customers.Models.Levels;
using LMS.Domain.Entities.HttpEntities;
using LMS.Domain.Financial.Models;
using LMS.Domain.HR.Models;
using LMS.Domain.Identity.Models;
using LMS.Domain.Identity.Models.Notifications;
using LMS.Domain.Identity.ValueObjects;
using LMS.Domain.LibraryManagement.Models;
using LMS.Domain.LibraryManagement.Models.Authors;
using LMS.Domain.LibraryManagement.Models.Categories;
using LMS.Domain.LibraryManagement.Models.Genres;
using LMS.Domain.LibraryManagement.Models.Products;
using LMS.Domain.LibraryManagement.Models.Publishers;
using LMS.Domain.LibraryManagement.Models.Relations;
using LMS.Domain.Orders.Models;
using LMS.Infrastructure.Configurations.Customers.Levels;
using LMS.Infrastructure.Configurations.Financial;
using LMS.Infrastructure.Configurations.HR;
using LMS.Infrastructure.Configurations.LibraryManagement;
using LMS.Infrastructure.Configurations.LibraryManagement.Authors;
using LMS.Infrastructure.Configurations.LibraryManagement.Categories;
using LMS.Infrastructure.Configurations.LibraryManagement.Genres;
using LMS.Infrastructure.Configurations.LibraryManagement.Products;
using LMS.Infrastructure.Configurations.LibraryManagement.Relations;
using LMS.Infrastructure.Configurations.Orders;
using LMS.Infrastructure.Configurations.Stock.Publishers;
using LMS.Infrastructure.Configurations.Users;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.DbContexts
{
    public class LMSDbContext : DbContext
    {
        // Users Namespace
        public DbSet<ImgurToken> ImgurTokens { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<EmployeeDepartment> EmployeeDepartments { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<OtpCode> OtpCodes { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationTranslation> NotificationTranslations { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }


        //HR Namespace
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Leave> Leaves { get; set; }
        public DbSet<LeaveBalance> LeaveBalances { get; set; }
        public DbSet<Salary> Salaries { get; set; }
        public DbSet<Incentive> Incentives { get; set; }
        public DbSet<Penalty> Penalties { get; set; }
        public DbSet<Holiday> Holidies { get; set; }


        // Orders Namespace
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Shipment> DeliveryOrders { get; set; }

        // Stock Namespace
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductsCategories { get; set; }
        public DbSet<ProductTranslation> ProductTranslations { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<GenreTranslation> GenreTranslations { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<PublisherTranslation> PublisherTranslations { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<AuthorTranslation> AuthorTranslations { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<GenreBook> GenreBooks { get; set; }
        public DbSet<PublisherBook> PublisherBooks { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryTranslation> categoryTranslations { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }

        public DbSet<Discount> Discounts { get; set; }
        public DbSet<InventoryLog> InventoryLogs { get; set; }

        //Financial Namespace:
        public DbSet<LoyaltyLevel> Levels { get; set; }
        public DbSet<LoyaltyLevelTransaltion> LevelTrnsaltions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Revenue> FinancialRevenues { get; set; }


        public LMSDbContext(DbContextOptions<LMSDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Users Namespace: 
            modelBuilder.ApplyConfiguration(new UserConfigurations());
            modelBuilder.ApplyConfiguration(new RoleConfigurations());
            modelBuilder.ApplyConfiguration(new EmployeeConfigurations());
            modelBuilder.ApplyConfiguration(new AddressConfigurations());
            modelBuilder.ApplyConfiguration(new RefreshTokenConfigurations());
            modelBuilder.ApplyConfiguration(new DepartmentConfigurations());
            modelBuilder.ApplyConfiguration(new EmployeeDepartmentConfigurations());
            modelBuilder.ApplyConfiguration(new OtpConfigurations());
            modelBuilder.ApplyConfiguration(new NotificationTranslationsConfigurations());
            modelBuilder.ApplyConfiguration(new NotificationTranslationsConfigurations());


            //Stock Namespace: 
            modelBuilder.ApplyConfiguration(new DiscountConfigurations());
            modelBuilder.ApplyConfiguration(new InventoryLogConfigurations());
            modelBuilder.ApplyConfiguration(new GenreConfigurations());
            modelBuilder.ApplyConfiguration(new GenreTranslationConfigurations());
            modelBuilder.ApplyConfiguration(new PublisherConfigurations());
            modelBuilder.ApplyConfiguration(new PublisherTranslationConfigurations());
            modelBuilder.ApplyConfiguration(new ProductConfigurations());
            modelBuilder.ApplyConfiguration(new ProductCategoryConfigurations());
            modelBuilder.ApplyConfiguration(new ProductTranslationConfigurations());
            modelBuilder.ApplyConfiguration(new BookConfigurations());
            modelBuilder.ApplyConfiguration(new GenreBookConfigurations());
            modelBuilder.ApplyConfiguration(new PublisherBookConfigurations());
            modelBuilder.ApplyConfiguration(new AuthorConfigurations());
            modelBuilder.ApplyConfiguration(new AuthorTranslationConfigurations());
            modelBuilder.ApplyConfiguration(new CategoryConfigurations());
            modelBuilder.ApplyConfiguration(new CategoryTranslationConfigurations());
            
            //Orders Namespace: 
            modelBuilder.ApplyConfiguration(new OrderConfigurations());
            modelBuilder.ApplyConfiguration(new OrderItemConfigurations());
            modelBuilder.ApplyConfiguration(new ShipmentConfigurations());
            modelBuilder.ApplyConfiguration(new CartItemConfigurations());
            modelBuilder.ApplyConfiguration(new CartConfigurations());

            //HR Namespace:
            modelBuilder.ApplyConfiguration(new SalaryConfigurations());
            modelBuilder.ApplyConfiguration(new PenaltyConfigurations());
            modelBuilder.ApplyConfiguration(new LeaveConfigurations());
            modelBuilder.ApplyConfiguration(new LeaveBalanceConfigurations());
            modelBuilder.ApplyConfiguration(new IncentiveConfigurations());
            modelBuilder.ApplyConfiguration(new AttendanceConfigurations());

            //Financial Namespace: 
            modelBuilder.ApplyConfiguration(new PaymentConfigurations());
            modelBuilder.ApplyConfiguration(new LevelConfigurations());
            modelBuilder.ApplyConfiguration(new LevelTranslationConfigurations());
            modelBuilder.ApplyConfiguration(new RevenueConfigurations());
        }
    }
}
