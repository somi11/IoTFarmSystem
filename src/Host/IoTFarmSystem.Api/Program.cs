using IoTFarmSystem.Api.Authorization.Polices;
using IoTFarmSystem.Api.Services;
using IoTFarmSystem.SharedKernel.Abstractions;
using IoTFarmSystem.SharedKernel.Security;
using IoTFarmSystem.UserManagement.Application;
using IoTFarmSystem.UserManagement.Infrastructure;
using IoTFarmSystem.UserManagement.Infrastructure.Identity;
using IoTFarmSystem.UserManagement.Infrastructure.Persistance;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --------------------- Configuration ---------------------
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// --------------------- Application & Infrastructure ---------------------
builder.Services.AddApplication();    // MediatR, Handlers
builder.Services.AddInfrastructure(); // Repos, Services, Identity
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.ConfigureAuthorizationPolicies();
//builder.Services.AddSingleton<IAuthorizationHandler, RoleRequirementHandler>();
//builder.Services.AddSingleton<IAuthorizationHandler, PermissionRequirementHandler>();
// --------------------- Controllers ---------------------
builder.Services.AddControllers();

// --------------------- JWT Authentication ---------------------
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero,
        RoleClaimType = "role",
        NameClaimType = ClaimTypes.NameIdentifier
    };
});

// --------------------- Swagger ---------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "IoTFarmSystem API", Version = "v1" });

    // JWT Auth in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {token}'"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference= new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// --------------------- Build App ---------------------
var app = builder.Build();

// --------------------- Seed Data ---------------------
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // Seed domain data
    var dbContext = services.GetRequiredService<UserManagementDbContext>();
    await UserManagementDbSeeder.SeedAsync(dbContext);

    // Seed Identity admin user
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    var adminEmail = "admin@iotfarm.com";
    var adminPassword = "Admin123!";
    var identityUser = await userManager.FindByEmailAsync(adminEmail);

    if (identityUser == null)
    {
        identityUser = new IdentityUser { UserName = adminEmail, Email = adminEmail };
        await userManager.CreateAsync(identityUser, adminPassword);
    }

    // Ensure SystemAdmin role exists
    if (!await roleManager.RoleExistsAsync(SystemRoles.SYSTEM_ADMIN))
    {
        await roleManager.CreateAsync(new IdentityRole(SystemRoles.SYSTEM_ADMIN));
    }

    // Assign role
    await userManager.AddToRoleAsync(identityUser, SystemRoles.SYSTEM_ADMIN);

    // Assign all claims (permissions) to admin
    var allPermissions = typeof(SystemPermissions)
                         .GetFields()
                         .Select(f => f.GetValue(null)?.ToString())
                         .Where(p => !string.IsNullOrEmpty(p));

    foreach (var perm in allPermissions)
    {
        if (!(await userManager.GetClaimsAsync(identityUser))
              .Any(c => c.Type == "permission" && c.Value == perm))
        {
            await userManager.AddClaimAsync(identityUser, new System.Security.Claims.Claim("permission", perm));
        }
    }
}

// --------------------- Middleware ---------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
