using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IoTFarmSystem.UserManagement.Infrastructure.Identity
{
    public class AppIdentityDbContext
        : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Prefix all Identity tables with "Identity_"
            builder.Entity<IdentityUser>(b => b.ToTable("Identity_Users"));
            builder.Entity<IdentityRole>(b => b.ToTable("Identity_Roles"));
            builder.Entity<IdentityUserRole<string>>(b => b.ToTable("Identity_UserRoles"));
            builder.Entity<IdentityUserClaim<string>>(b => b.ToTable("Identity_UserClaims"));
            builder.Entity<IdentityUserLogin<string>>(b => b.ToTable("Identity_UserLogins"));
            builder.Entity<IdentityRoleClaim<string>>(b => b.ToTable("Identity_RoleClaims"));
            builder.Entity<IdentityUserToken<string>>(b => b.ToTable("Identity_UserTokens"));
        }
    }
}
