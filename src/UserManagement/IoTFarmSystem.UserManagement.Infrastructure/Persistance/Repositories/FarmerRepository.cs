using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using IoTFarmSystem.UserManagement.Application.DTOs;
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

        public async Task<Farmer?> GetEntityByIdAsync(Guid farmerId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Farmers
                .FirstOrDefaultAsync(f => f.Id == farmerId, cancellationToken);
        }
        public async Task<FarmerDto?> GetByIdAsync(Guid farmerId, CancellationToken cancellationToken = default)
        {
            var farmer = await _dbContext.Farmers
                .Include(f => EF.Property<List<UserRole>>(f, "_roles"))
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => EF.Property<List<RolePermission>>(r, "_permissions"))
                            .ThenInclude(rp => rp.Permission)  
                .Include(f => EF.Property<List<UserPermission>>(f, "_permissions"))
                .FirstOrDefaultAsync(f => f.Id == farmerId, cancellationToken);

            if (farmer == null)
                return null;

            // Effective = role permissions ∪ explicit permissions
            var rolePermissions = farmer.Roles
                .SelectMany(r => r.Role.Permissions)
                .Select(p => p.Permission.Name);

            var explicitPermissions = farmer.ExplicitPermissions
                .Select(p => p.PermissionName);

            var effectivePermissions = rolePermissions
                .Union(explicitPermissions)   // remove duplicates
                .ToList();

            return new FarmerDto
            {
                Id = farmer.Id,
                TenantId = farmer.TenantId,
                Email = farmer.Email,
                Name = farmer.Name,
                Roles = farmer.Roles.Select(r => r.Role.Name).ToList(),
                Permissions = effectivePermissions
            };
        }

        public async Task<FarmerDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var farmer = await _dbContext.Farmers
                .Include(f => EF.Property<List<UserRole>>(f, "_roles"))
                    .ThenInclude(ur => ur.Role)
                .Include(f => EF.Property<List<UserPermission>>(f, "_permissions"))
                .FirstOrDefaultAsync(f => f.Email == email, cancellationToken);

            if (farmer == null)
                return null;

            return new FarmerDto
            {
                Id = farmer.Id,
                TenantId = farmer.TenantId,
                Email = farmer.Email,
                Name = farmer.Name,
                Roles = farmer.Roles.Select(r => r.Role.Name).ToList(),
                Permissions = farmer.ExplicitPermissions.Select(p => p.PermissionName).ToList()
            };
        }


        public async Task<IReadOnlyList<FarmerDto>> GetByTenantAsync(
            Guid tenantId,
            CancellationToken cancellationToken = default)
        {
            var farmers = await _dbContext.Farmers
                .Where(f => f.TenantId == tenantId)
                .Include(f => EF.Property<List<UserRole>>(f, "_roles"))
                    .ThenInclude(ur => ur.Role)
                .Include(f => EF.Property<List<UserPermission>>(f, "_permissions"))
                    .ThenInclude(up => up.Permission)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return farmers.Select(f => new FarmerDto
            {
                Id = f.Id,
                TenantId = f.TenantId,
                Email = f.Email,
                Name = f.Name,
                Roles = f.Roles
                    .Select(r => r.Role?.Name)
                    .Where(n => !string.IsNullOrEmpty(n))
                    .Distinct()
                    .ToList(),
                Permissions = f.ExplicitPermissions
                    .Select(p => p.PermissionName)
                    .Where(n => !string.IsNullOrEmpty(n))
                    .Distinct()
                    .ToList()
            }).ToList();
        }
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

        public async Task<IReadOnlyList<FarmerDto>> GetByRoleNameAsync(
            Guid tenantId,
            string roleName,
            CancellationToken cancellationToken = default)
        {
            // Step 1: get farmer ids via a SQL-translatable join (UserRole -> Role -> Farmer)
            var farmerIds = await (
                from ur in _dbContext.Set<UserRole>()
                join r in _dbContext.Set<Role>() on ur.RoleId equals r.Id
                join f in _dbContext.Farmers on ur.UserId equals f.Id
                where r.Name == roleName && f.TenantId == tenantId
                select f.Id
            )
            .Distinct()
            .ToListAsync(cancellationToken);

            if (!farmerIds.Any())
                return Array.Empty<FarmerDto>();

            // Step 2: load farmers with backing-field includes (populate _roles/_permissions)
            var farmers = await _dbContext.Farmers
                .Where(f => farmerIds.Contains(f.Id))
                .Include(f => EF.Property<List<UserRole>>(f, "_roles"))
                    .ThenInclude(ur => ur.Role)
                .Include(f => EF.Property<List<UserPermission>>(f, "_permissions"))
                    .ThenInclude(up => up.Permission)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // Step 3: project to DTO in memory (safe and simple)
            var result = farmers.Select(f => new FarmerDto
            {
                Id = f.Id,
                TenantId = f.TenantId,
                Email = f.Email,
                Name = f.Name,
                Roles = f.Roles
                    .Select(rr => rr.Role?.Name)
                    .Where(n => !string.IsNullOrEmpty(n))
                    .Distinct()
                    .ToList(),
                Permissions = f.ExplicitPermissions
                    .Select(p => p.PermissionName)
                    .Where(n => !string.IsNullOrEmpty(n))
                    .Distinct()
                    .ToList(),
                CreatedAt = null,
                LastLogin = null
            }).ToList();

            return result;
        }

        public async Task<Farmer?> GetWithRolesAndPermissionsAsync(Guid farmerId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Farmers
                .Include(f => EF.Property<List<UserRole>>(f, "_roles"))
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => EF.Property<List<RolePermission>>(r, "_permissions")) // include role permissions
                .Include(f => EF.Property<List<UserPermission>>(f, "_permissions"))
                    .ThenInclude(up => up.Permission)
                .FirstOrDefaultAsync(f => f.Id == farmerId, cancellationToken);
        }

        // ------------------ Mutating methods (no SaveChanges here) ------------------

        public Task AddAsync(Farmer farmer, CancellationToken cancellationToken = default)
        {
            return _dbContext.Farmers.AddAsync(farmer, cancellationToken).AsTask();
        }

        public Task UpdateAsync(Farmer farmer, CancellationToken cancellationToken = default)
        {
            _dbContext.Farmers.Update(farmer);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Farmer farmer, CancellationToken cancellationToken = default)
        {
            _dbContext.Farmers.Remove(farmer);
            return Task.CompletedTask;
        }

        public Task AssignRoleAsync(Farmer farmer, Role role, CancellationToken cancellationToken = default)
        {
            farmer.AssignRole(role);
            _dbContext.Farmers.Update(farmer);
            return Task.CompletedTask;
        }

        public Task RevokeRoleAsync(Farmer farmer, Role role, CancellationToken cancellationToken = default)
        {
            farmer.RevokeRole(role);
            _dbContext.Farmers.Update(farmer);
            return Task.CompletedTask;
        }

        public Task GrantPermissionAsync(Farmer farmer, Permission permission, CancellationToken cancellationToken = default)
        {
            farmer.GrantPermission(permission);
            _dbContext.Farmers.Update(farmer);
            return Task.CompletedTask;
        }

        public Task RevokePermissionAsync(Farmer farmer, Permission permission, CancellationToken cancellationToken = default)
        {
            farmer.RevokePermission(permission.Id);
            _dbContext.Farmers.Update(farmer);
            return Task.CompletedTask;
        }
    }
}
