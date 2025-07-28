using System.Text;
using System.Text.Json.Serialization;
using LMS.API.Hubs.Notifications;
using LMS.API.Middlewares;
using LMS.API.Services;
using LMS.Application.Abstractions.Accounting;
using LMS.Application.Abstractions.Authentication;
using LMS.Application.Abstractions.Common;
using LMS.Application.Abstractions.Communications;
using LMS.Application.Abstractions.Customers;
using LMS.Application.Abstractions.HR;
using LMS.Application.Abstractions.Identity;
using LMS.Application.Abstractions.ImagesServices;
using LMS.Application.Abstractions.LibraryManagement;
using LMS.Application.Abstractions.Loggings;
using LMS.Application.Abstractions.Reports;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.Abstractions.Services.Admin;
using LMS.Application.Abstractions.Services.Authentication;
using LMS.Application.Abstractions.Services.Helpers;
using LMS.Application.Abstractions.UnitOfWorks;
using LMS.Application.Features.Authentication.Register.Commands.CreateTempAccount;
using LMS.Application.Settings;
using LMS.Common.Settings;
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
using LMS.Infrastructure.DbContexts;
using LMS.Infrastructure.Helpers;
using LMS.Infrastructure.Helpers.Accounting;
using LMS.Infrastructure.Helpers.Admin;
using LMS.Infrastructure.Helpers.Common;
using LMS.Infrastructure.Helpers.Communications;
using LMS.Infrastructure.Helpers.HR;
using LMS.Infrastructure.Helpers.identity;
using LMS.Infrastructure.Helpers.LibraryManagement;
using LMS.Infrastructure.Helpers.Loggings;
using LMS.Infrastructure.Helpers.Reports;
using LMS.Infrastructure.Repositories.Customers;
using LMS.Infrastructure.Repositories.Customers.Levels;
using LMS.Infrastructure.Repositories.Financial;
using LMS.Infrastructure.Repositories.HR;
using LMS.Infrastructure.Repositories.Identity;
using LMS.Infrastructure.Repositories.LibraryManagement;
using LMS.Infrastructure.Repositories.LibraryManagement.Authors;
using LMS.Infrastructure.Repositories.LibraryManagement.Categories;
using LMS.Infrastructure.Repositories.LibraryManagement.Genres;
using LMS.Infrastructure.Repositories.LibraryManagement.Products;
using LMS.Infrastructure.Repositories.LibraryManagement.Publishers;
using LMS.Infrastructure.Repositories.LibraryManagement.Relations;
using LMS.Infrastructure.Repositories.Orders;
using LMS.Infrastructure.Services.Authentication;
using LMS.Infrastructure.Services.Authentication.Token;
using LMS.Infrastructure.Services.Helpers;
using LMS.Infrastructure.UnitOfWorks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using Serilog;

namespace LMS.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Warning()
                .WriteTo.Console()
                .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Month)
                .CreateLogger();

            var builder = WebApplication.CreateBuilder(args);


            //Token And Authentication configure:
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");

            builder.Services.AddAuthentication("Bearer").AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,


                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!)),
                    ClockSkew = TimeSpan.Zero
                };
            });


            //Enum to string conerter in the http responses:
            builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });


            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            //Inject the DbContext:
            builder.Services.AddDbContext<LMSDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );


            //Applay Versioning:
            builder.Services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;


                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            });


            builder.Services.AddHttpClient();


            // Inject the Repositories:
            // Users Repositories:
            builder.Services.AddScoped<IBaseRepository<ImgurToken>, ImgurTokenRepository>();
            builder.Services.AddScoped<ISoftDeletableRepository<Role>, RoleRepository>();
            builder.Services.AddScoped<ISoftDeletableRepository<User>, UserRepositroy>();
            builder.Services.AddScoped<ISoftDeletableRepository<Department>, DepartmentRepository>();
            builder.Services.AddScoped<ISoftDeletableRepository<Employee>, EmployeeRepository>();
            builder.Services.AddScoped<ISoftDeletableRepository<EmployeeDepartment>, EmployeeDepartmentRepository>();
            builder.Services.AddScoped<ISoftDeletableRepository<Customer>, CustomerRepository>();
            builder.Services.AddScoped<ISoftDeletableRepository<Address>, AddressRepository>();
            builder.Services.AddScoped<IBaseRepository<OtpCode>, OtpCodeRepository>();
            builder.Services.AddScoped<IBaseRepository<Notification>, NotificationRepository>();
            builder.Services.AddScoped<IBaseRepository<NotificationTranslation>, NotificationTranslationRepository>();
            builder.Services.AddScoped<IBaseRepository<RefreshToken>, RefreshTokenRepository>();


            //Stock Repositories: 
            builder.Services.AddScoped<ISoftDeletableRepository<Publisher>, PublisherRepository>();
            builder.Services.AddScoped<IBaseRepository<PublisherTranslation>, PublisherTranslationRepository>();
            builder.Services.AddScoped<ISoftDeletableRepository<Product>, ProductRepository>();
            builder.Services.AddScoped<IBaseRepository<ProductTranslation>, ProductTranslationRepository>();
            builder.Services.AddScoped<IBaseRepository<InventoryLog>, InventoryLogRepository>();
            builder.Services.AddScoped<ISoftDeletableRepository<Genre>, GenreRepository>();
            builder.Services.AddScoped<IBaseRepository<GenreTranslation>, GenreTranslationRepository>();
            builder.Services.AddScoped<ISoftDeletableRepository<Book>, BookRepository>();
            builder.Services.AddScoped<ISoftDeletableRepository<Author>, AuthorRepository>();
            builder.Services.AddScoped<IBaseRepository<AuthorTranslation>, AuthorTranslationRepository>();
            builder.Services.AddScoped<IBaseRepository<CategoryTranslation>, CategoryTranslationRepository>();
            builder.Services.AddScoped<ISoftDeletableRepository<Category>, CategoryRepository>();
            builder.Services.AddScoped<IBaseRepository<ProductCategory>, ProductCategoryRepository>();
            builder.Services.AddScoped<IBaseRepository<GenreBook>, GenreBookRepository>();
            builder.Services.AddScoped<IBaseRepository<PublisherBook>, PublisherBookRepository>();


            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
                cfg.RegisterServicesFromAssemblies(typeof(CreateTempAccountCommand).Assembly);
            });

            //Orders Repositories: 
            builder.Services.AddScoped<ISoftDeletableRepository<Order>, OrderRepository>();
            builder.Services.AddScoped<ISoftDeletableRepository<Shipment>, ShipmentRepository>();
            builder.Services.AddScoped<IBaseRepository<CartItem>, CartItemRepository>();
            builder.Services.AddScoped<ISoftDeletableRepository<Cart>, CartRepository>();
            builder.Services.AddScoped<ISoftDeletableRepository<OrderItem>, OrderItemRepository>();


            builder.Services.AddSingleton<IFaceRecognitionHelper, FaceRecognitionHelper>();

            //HR Repositories:
            builder.Services.AddScoped<ISoftDeletableRepository<Salary>, SalaryRepository>();
            builder.Services.AddScoped<ISoftDeletableRepository<Penalty>, PenaltyRepository>();
            builder.Services.AddScoped<ISoftDeletableRepository<Leave>, LeaveRepository>();
            builder.Services.AddScoped<IBaseRepository<LeaveBalance>, LeaveBalanceRepository>();
            builder.Services.AddScoped<ISoftDeletableRepository<Incentive>, IncentiveRepository>();
            builder.Services.AddScoped<ISoftDeletableRepository<Attendance>, AttendanceRepository>();
            builder.Services.AddScoped<IBaseRepository<Holiday>, HolidayRepository>();


            //Financial Repositories: 
            builder.Services.AddScoped<ISoftDeletableRepository<LoyaltyLevel>, LevelRepository>();
            builder.Services.AddScoped<IBaseRepository<LoyaltyLevelTransaltion>, LevelTranslationRepository>();
            builder.Services.AddScoped<ISoftDeletableRepository<Revenue>, RevenueRepository>();
            builder.Services.AddScoped<ISoftDeletableRepository<Payment>, PaymentRepository>();


            builder.Services.Configure<EmailSettings>(
                builder.Configuration.GetSection("EmailSettings"));


            builder.Services.Configure<TokenSettings>(
                builder.Configuration.GetSection("JwtSettings"));


            builder.Services.Configure<CloudinarySettings>(
                builder.Configuration.GetSection("CloudinarySettings"));


            builder.Services.Configure<FrontendSettings>(
                builder.Configuration.GetSection("Frontend"));



            //Application Services:
            builder.Services.AddScoped(typeof(IAppLogger<>), typeof(AppLogger<>));
            

            builder.Services.AddScoped<ITokenGeneratorService, TokenGeneratorService>();
            builder.Services.AddScoped<ITokenReaderService, TokenReaderService>();
            builder.Services.AddScoped<IEmailSenderService, EmailSenderService>();
            builder.Services.AddScoped<IEmailTemplateReaderService, EmailTemplateReaderService>();
            builder.Services.AddScoped<IRandomGeneratorService, RandomGeneratorService>();
            builder.Services.AddScoped<IConverterHelper, ConverterHelper>();



            builder.Services.AddScoped<IDashboardHelper, DashboardHelper>();
            builder.Services.AddScoped<IFinancialHelper, FinancialHelper>();
            builder.Services.AddScoped<IAuthenticationHelper, AuthenticationHelper>();
            builder.Services.AddScoped<IFileHostingUploaderHelper, FileHostingUploaderHelper>();
            builder.Services.AddScoped<IEmployeeHelper, EmployeeHelper>();
            builder.Services.AddScoped<IDepartmentHelper, DepartmentHelper>();
            builder.Services.AddScoped<IInventoryHelper, InventoryHelper>();
            builder.Services.AddScoped<ICustomerHelper, CustomerHelper>();
            builder.Services.AddScoped<IReportsGeneratorHelper, ReportsGeneratorHelper>();
            builder.Services.AddScoped<IEmployeeCompensationHelper, EmployeeCompensationHelper>();
            builder.Services.AddScoped<INotificationHelper, NotificationHelper>();
            builder.Services.AddScoped<IProductHelper, ProductHelper>();
            builder.Services.AddScoped<IStockManagementHelper, StockManagementHelper>();

            //HR




            //Adding Crose For Fronend Application:
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp",
                    policy => policy
                        .WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                );
            });



            //Domain Services:
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


            builder.Services.AddSignalR();


            //Adding AutoMapper and its profiles:
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


            //Scheduled Jobs:
            builder.Services.AddQuartz(q =>
            {
                var jobKey = new JobKey("DailyAttendanceJob");

                q.AddJob<AttendanceGenerationJob>(opts => opts.WithIdentity(jobKey));
                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity("DailyAttendanceTrigger")
                    .WithCronSchedule("0 0 21 * * ?")
                );
            });

            builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);


            builder.Host.UseSerilog();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowAngularApp");


            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.MapControllers();

            app.MapHub<NotificationHub>("/notificationHub");

            app.Run();
        }
    }
}
