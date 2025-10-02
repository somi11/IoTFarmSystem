using IoTFarmSystem.UserManagement.Application.Contracts.Authorizatioon;
using IoTFarmSystem.UserManagement.Application.Contracts.Identity;
using IoTFarmSystem.UserManagement.Application.Contracts.Persistance;
using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using IoTFarmSystem.UserManagement.Application.Contracts.Services;
using IoTFarmSystem.UserManagement.Domain.Entites;
using IoTFarmSystem.UserManagement.Infrastructure.Identity;
using IoTFarmSystem.UserManagement.Infrastructure.Persistance;
using IoTFarmSystem.UserManagement.Infrastructure.Persistance.Repositories;
using IoTFarmSystem.UserManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // Domain context
            //services.AddDbContext<UserManagementDbContext>(options =>
            //    options.UseInMemoryDatabase("UserManagementDb"));

            services.AddDbContext<UserManagementDbContext>(options =>
            {
                options.UseInMemoryDatabase("UserManagementDb")
                       .EnableSensitiveDataLogging()
                       .EnableDetailedErrors()
                       .UseLoggerFactory(LoggerFactory.Create(builder =>
                       {
                           builder.AddDebug();      // Output window
                           builder.AddConsole();    // Console
                       }))
                       .LogTo(Console.WriteLine, LogLevel.Information);
            });
            // Identity context
            services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseInMemoryDatabase("IdentityDb"));

            // Identity setup
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<AppIdentityDbContext>()
            .AddDefaultTokenProviders();

            // Application/Domain services
            services.AddScoped<IUserService, UserService>();      // Custom user manager
            services.AddScoped<IAuthService, AuthService>();      // Authentication service
            services.AddScoped<IAuthorizationClaimsProvider,AuthorizationClaimsProvider>(); // Custom claims provider
            services.AddScoped<IJwtService, JwtService>();        // JWT generator
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPermissionLookupService, PermissionLookupService>();

            // Repositories
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IFarmerRepository, FarmerRepository>();
            services.AddScoped<ITenantRepository, TenantRepository>();

            return services;

        }
    }
}
