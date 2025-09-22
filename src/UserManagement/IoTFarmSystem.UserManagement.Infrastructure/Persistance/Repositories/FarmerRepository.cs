using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using IoTFarmSystem.UserManagement.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace IoTFarmSystem.UserManagement.Infrastructure.Persistance.Repositories
{
    public class FarmerRepository : IFarmerRepository
    {
        private readonly UserManagementDbContext _dbContext;

        public FarmerRepository(UserManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Farmer?> GetByIdAsync(Guid farmerId, CancellationToken cancellationToken = default)
            => await _dbContext.Farmers.FindAsync(new object[] { farmerId }, cancellationToken);

        public async Task<Farmer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
            => await _dbContext.Farmers.FirstOrDefaultAsync(f => f.Email == email, cancellationToken);

        public async Task<IReadOnlyList<Farmer>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
            => await _dbContext.Farmers.Where(f => f.TenantId == tenantId).ToListAsync(cancellationToken);
        public async Task<Farmer?> GetWithRolesAsync(Guid farmerId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Farmers
                .Include(f => EF.Property<List<UserRole>>(f, "_roles"))
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(f => f.Id == farmerId, cancellationToken);
        }

        public async Task<Farmer?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Farmers
                .Include(f => EF.Property<List<UserRole>>(f, "_roles"))
                    .ThenInclude(ur => ur.Role)
                .Include(f => EF.Property<List<UserPermission>>(f, "_permissions"))
                .FirstOrDefaultAsync(f => f.IdentityUserId == identityUserId, cancellationToken);
        }

        public async Task<Farmer?> GetWithPermissionsAsync(Guid farmerId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Farmers
                .Include(f => EF.Property<List<UserPermission>>(f, "_permissions"))
                    .ThenInclude(up => up.Permission)
                .FirstOrDefaultAsync(f => f.Id == farmerId, cancellationToken);
        }

        public async Task<IReadOnlyList<Farmer>> GetByRoleAsync(string roleName, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Farmers
                .Include(f => EF.Property<List<UserRole>>(f, "_roles"))
                    .ThenInclude(ur => ur.Role)
                .Where(f => f.Roles.Any(r => r.Role.Name == roleName))
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Farmer farmer, CancellationToken cancellationToken = default)
        {
            await _dbContext.Farmers.AddAsync(farmer, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Farmer farmer, CancellationToken cancellationToken = default)
        {
            _dbContext.Farmers.Update(farmer);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Farmer farmer, CancellationToken cancellationToken = default)
        {
            _dbContext.Farmers.Remove(farmer);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public Task AssignRoleAsync(Farmer farmer, Role role, CancellationToken cancellationToken = default)
        {
            farmer.AssignRole(role);
            return UpdateAsync(farmer, cancellationToken);
        }

        public Task RevokeRoleAsync(Farmer farmer, Role role, CancellationToken cancellationToken = default)
        {
            farmer.RevokeRole(role);
            return UpdateAsync(farmer, cancellationToken);
        }

        public Task GrantPermissionAsync(Farmer farmer, Permission permission, CancellationToken cancellationToken = default)
        {
            farmer.GrantPermission(permission);
            return UpdateAsync(farmer, cancellationToken);
        }

        public Task RevokePermissionAsync(Farmer farmer, Permission permission, CancellationToken cancellationToken = default)
        {
            farmer.RevokePermission(permission.Name);
            return UpdateAsync(farmer, cancellationToken);
        }
    }
}
