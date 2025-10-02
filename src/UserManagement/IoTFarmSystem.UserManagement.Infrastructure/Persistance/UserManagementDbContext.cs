using IoTFarmSystem.UserManagement.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace IoTFarmSystem.UserManagement.Infrastructure.Persistance
{
    public class UserManagementDbContext : DbContext
    {
        public UserManagementDbContext(DbContextOptions<UserManagementDbContext> options)
            : base(options) { }

        // Aggregates
        public DbSet<Farmer> Farmers { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }

        // Junction tables
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .EnableSensitiveDataLogging() // shows parameter values
                    .EnableDetailedErrors()       // more info in exceptions
                    .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========================
            // Farmer
            // ========================
            modelBuilder.Entity<Farmer>(entity =>
            {
                entity.HasKey(f => f.Id);

                entity.Property(f => f.Name)
                      .IsRequired(false);

                entity.Property(f => f.Email)
                      .IsRequired();

                entity.Property(f => f.IdentityUserId)
                      .IsRequired();

                entity.HasMany<UserRole>("_roles")
                  .WithOne()
                  .HasForeignKey(ur => ur.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany<UserPermission>("_permissions")
                      .WithOne()
                      .HasForeignKey(up => up.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Backing fields
                entity.Navigation("_roles").UsePropertyAccessMode(PropertyAccessMode.Field);
                entity.Navigation("_permissions").UsePropertyAccessMode(PropertyAccessMode.Field);

                // Ignore public navigation properties to avoid mapping conflict
                entity.Ignore(f => f.Roles);
                entity.Ignore(f => f.ExplicitPermissions);
            });

            // ========================
            // Tenant
            // ========================
            modelBuilder.Entity<Tenant>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Name)
                      .IsRequired();

                entity.HasMany<Farmer>("_farmers")
                      .WithOne()
                      .HasForeignKey(f => f.TenantId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Navigation("_farmers").UsePropertyAccessMode(PropertyAccessMode.Field);

                // Ignore public navigation property to avoid mapping conflict
                entity.Ignore(t => t.Farmers);
            });

            // ========================
            // Role
            // ========================
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.Property(r => r.Name)
                      .IsRequired();

                entity.HasMany<RolePermission>("_permissions")
                      .WithOne()
                      .HasForeignKey(rp => rp.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Navigation("_permissions").UsePropertyAccessMode(PropertyAccessMode.Field);

                // Ignore public navigation property to avoid mapping conflict
                entity.Ignore(r => r.Permissions);
            });

            // ========================
            // Permission
            // ========================
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired();
            });

            // ========================
            // Junction entities
            // ========================
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(ur => new { ur.UserId, ur.RoleId });
            });

            modelBuilder.Entity<UserPermission>(entity =>
            {
                entity.HasKey(up => new { up.UserId, up.PermissionId });
            });

            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(rp => new { rp.RoleId, rp.PermissionId });
            });
        }
    }
}
