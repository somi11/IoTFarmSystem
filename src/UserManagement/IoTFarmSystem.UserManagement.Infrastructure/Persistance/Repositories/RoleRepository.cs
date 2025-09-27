using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using IoTFarmSystem.UserManagement.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace IoTFarmSystem.UserManagement.Infrastructure.Persistance.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly UserManagementDbContext _dbContext;

        public RoleRepository(UserManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // ========================
        // Basic queries
        // ========================
        public async Task<Role?> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Roles
                .Include(r => EF.Property<List<RolePermission>>(r, "_permissions"))
                    .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);
        }

        public async Task<Role?> GetByNameAsync(string roleName, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Roles
                .Include(r => EF.Property<List<RolePermission>>(r, "_permissions"))
                    .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Name == roleName, cancellationToken);
        }

        public async Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Roles
                .Include(r => EF.Property<List<RolePermission>>(r, "_permissions"))
                    .ThenInclude(rp => rp.Permission)
                .ToListAsync(cancellationToken);
        }

        // ========================
        // CRUD (no SaveChanges here!)
        // ========================
        public async Task AddAsync(Role role, CancellationToken cancellationToken = default)
        {
            await _dbContext.Roles.AddAsync(role, cancellationToken);
        }

        public Task UpdateAsync(Role role, CancellationToken cancellationToken = default)
        {
            if (_dbContext.Entry(role).State == EntityState.Detached)
            {
                _dbContext.Roles.Update(role);
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Role role, CancellationToken cancellationToken = default)
        {
            _dbContext.Roles.Remove(role);
            return Task.CompletedTask;
        }

        // ========================
        // Domain-level permission management
        // ========================
        public Task AddPermissionAsync(Role role, Permission permission, CancellationToken cancellationToken = default)
        {
            if (_dbContext.Entry(role).State == EntityState.Detached)
            {
                _dbContext.Roles.Attach(role);
            }

            role.AddPermission(permission); // modifies backing field
            return Task.CompletedTask; // SaveChanges deferred to UnitOfWork
        }

        public Task RemovePermissionAsync(Role role, Permission permission, CancellationToken cancellationToken = default)
        {
            var existing = EF.Property<List<RolePermission>>(role, "_permissions")
                              .FirstOrDefault(p => p.PermissionId == permission.Id);

            if (existing != null)
            {
                _dbContext.RolePermissions.Remove(existing);
            }

            return Task.CompletedTask; // SaveChanges deferred to UnitOfWork
        }
    }
}
