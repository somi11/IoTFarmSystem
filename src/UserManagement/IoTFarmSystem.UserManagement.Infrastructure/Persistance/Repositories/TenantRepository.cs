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
                .Include(t => t.Farmers)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public async Task<Tenant?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tenants
                .Include(t => t.Farmers)
                .FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
        }

        public async Task<IReadOnlyList<Tenant>> GetAllWithFarmersAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tenants
                .Include(t => t.Farmers)
                .ToListAsync(cancellationToken);
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
                .Where(f => f.TenantId == tenantId && f.Roles.Any(r => _dbContext.Roles.Any(role => role.Id == r.RoleId && role.Name == roleName)))
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Farmer>> GetFarmersByPermissionAsync(Guid tenantId, string permissionName, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Farmers
                .Where(f => f.TenantId == tenantId && f.Permissions.Any(p => p.PermissionName == permissionName))
                .ToListAsync(cancellationToken);
        }

        // ============================
        // CRUD operations
        // ============================

        public async Task AddAsync(Tenant tenant, CancellationToken cancellationToken = default)
        {
            await _dbContext.Tenants.AddAsync(tenant, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Tenant tenant, CancellationToken cancellationToken = default)
        {
            _dbContext.Tenants.Update(tenant);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Tenant tenant, CancellationToken cancellationToken = default)
        {
            _dbContext.Tenants.Remove(tenant);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        // ============================
        // Domain-level farmer ops
        // ============================

        public async Task AddFarmerAsync(Tenant tenant, Farmer farmer, CancellationToken cancellationToken = default)
        {
            tenant.RegisterFarmer(farmer.IdentityUserId, farmer.Email);
            _dbContext.Tenants.Update(tenant);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveFarmerAsync(Tenant tenant, Farmer farmer, CancellationToken cancellationToken = default)
        {
            var existingFarmer = tenant.Farmers.FirstOrDefault(f => f.Id == farmer.Id);
            if (existingFarmer != null)
            {
                _dbContext.Farmers.Remove(existingFarmer);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
