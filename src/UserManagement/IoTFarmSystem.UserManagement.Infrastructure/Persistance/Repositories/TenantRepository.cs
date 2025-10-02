using IoTFarmSystem.UserManagement.Application.DTOs;
using IoTFarmSystem.UserManagement.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace IoTFarmSystem.UserManagement.Infrastructure.Persistance.Repositories
{
    public class TenantRepository : ITenantRepository
    {
        private readonly UserManagementDbContext _dbContext;

        public TenantRepository(UserManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // ============================
        // Basic tenant queries
        // ============================

        public async Task<Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tenants
                .Include(t => EF.Property<List<Farmer>>(t, "_farmers"))
                    .ThenInclude(f => EF.Property<List<UserRole>>(f, "_roles"))
                        .ThenInclude(ur => ur.Role)
                .Include(t => EF.Property<List<Farmer>>(t, "_farmers"))
                    .ThenInclude(f => EF.Property<List<UserPermission>>(f, "_permissions"))
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }
        public async Task<TenantDto?> GetByIdQueryAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var tenant = await _dbContext.Tenants
                .Where(t => t.Id == id)
                .Select(t => new TenantDto
            {
                Id = t.Id,
                Name = t.Name,
                Farmers = EF.Property<List<Farmer>>(t, "_farmers")
                .Select(f => new FarmerSummaryDto
                {
                    Id = f.Id,
                    Email = f.Email,
                    Name = f.Name
                }).ToList()
            }).FirstOrDefaultAsync(cancellationToken);
            return tenant;
        }

public async Task<TenantDto?> GetByNameQueryAsync(string name, CancellationToken cancellationToken = default)
        {
            var tenant = await _dbContext.Tenants
                .Where(t => t.Name == name)
                .Select(t => new TenantDto
            {
                Id = t.Id,
                Name = t.Name,
                Farmers = EF.Property<List<Farmer>>(t, "_farmers")
                .Select(f => new FarmerSummaryDto
                {
                    Id = f.Id,
                    Email = f.Email,
                    Name = f.Name
                }).ToList()
            }).FirstOrDefaultAsync(cancellationToken);
            return tenant;
        }

        public async Task<IReadOnlyList<Tenant>> GetAllWithFarmersAsync(CancellationToken cancellationToken = default)
        {
            //return await _dbContext.Tenants
            //    .Include(t => t.Farmers)
            //    .ToListAsync(cancellationToken);
            return await _dbContext.Tenants
                .Include(t => EF.Property<List<Farmer>>(t, "_farmers")).ToListAsync(cancellationToken);
                    
        }
        public async Task<IReadOnlyList<TenantDto>> GetAllWithFarmersQueryAsync(CancellationToken cancellationToken = default)
        {
            //return await _dbContext.Tenants
            //    .Include(t => t.Farmers)
            //    .ToListAsync(cancellationToken);
            var tenants = await _dbContext.Tenants.Select(t => new TenantDto
            {
                Id = t.Id,
                Name = t.Name,
                Farmers = EF.Property<List<Farmer>>(t, "_farmers")
                .Select(f => new FarmerSummaryDto
                {
                    Id = f.Id,
                    Email = f.Email,
                    Name = f.Name
                }).ToList()
            }).ToListAsync(cancellationToken);
            return tenants;
        }
        // ============================
        // Tenant queries by farmer
        // ============================

        public async Task<Tenant?> GetByFarmerIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tenants
                .Include(t => t.Farmers)
                .FirstOrDefaultAsync(t => t.Farmers.Any(f => f.IdentityUserId == identityUserId), cancellationToken);
        }

        public async Task<IReadOnlyList<Farmer>> GetFarmersByRoleAsync(Guid tenantId, string roleName, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Farmers
                .Where(f => f.TenantId == tenantId &&
                            f.Roles.Any(r => _dbContext.Roles.Any(role => role.Id == r.RoleId && role.Name == roleName)))
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Farmer>> GetFarmersByPermissionAsync(Guid tenantId, string permissionName, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Farmers
                .Where(f => f.TenantId == tenantId &&
                            f.ExplicitPermissions.Any(p => p.PermissionName == permissionName))
                .ToListAsync(cancellationToken);
        }

        // ============================
        // CRUD operations (no SaveChanges here)
        // ============================

        public Task AddAsync(Tenant tenant, CancellationToken cancellationToken = default)
        {
            return _dbContext.Tenants.AddAsync(tenant, cancellationToken).AsTask();
        }

        public Task UpdateAsync(Tenant tenant, CancellationToken cancellationToken = default)
        {
            _dbContext.Tenants.Update(tenant);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Tenant tenant, CancellationToken cancellationToken = default)
        {
            _dbContext.Tenants.Remove(tenant);
            return Task.CompletedTask;
        }

        // ============================
        // Domain-level farmer ops
        // ============================

        public Task AddFarmerAsync(Tenant tenant, Farmer farmer, CancellationToken cancellationToken = default)
        {
            tenant.RegisterFarmer(Guid.NewGuid(), farmer.IdentityUserId, farmer.Email, farmer.Name);
            _dbContext.Tenants.Update(tenant);
            return Task.CompletedTask;
        }

        public Task RemoveFarmerAsync(Tenant tenant, Farmer farmer, CancellationToken cancellationToken = default)
        {
            var existingFarmer = tenant.Farmers.FirstOrDefault(f => f.Id == farmer.Id);
            if (existingFarmer != null)
            {
                _dbContext.Farmers.Remove(existingFarmer);
            }
            return Task.CompletedTask;
        }
    }
}
