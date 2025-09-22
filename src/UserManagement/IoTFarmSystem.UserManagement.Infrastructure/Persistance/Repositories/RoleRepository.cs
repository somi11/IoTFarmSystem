using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using IoTFarmSystem.UserManagement.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using System.Data;

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
                 .Include("_permissions")
                .FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);
        }

        public async Task<Role?> GetByNameAsync(string roleName, CancellationToken cancellationToken = default)
        {
            return  await _dbContext.Roles
            .Include("_permissions") // use string name of backing field
            .FirstOrDefaultAsync(r => r.Name == roleName, cancellationToken);
        }

        public async Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Roles
                .Include("_permissions")
                .ToListAsync(cancellationToken);
        }

        // ========================
        // CRUD
        // ========================
        public async Task AddAsync(Role role, CancellationToken cancellationToken = default)
        {
            await _dbContext.Roles.AddAsync(role, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Role role, CancellationToken cancellationToken = default)
        {
            _dbContext.Roles.Update(role);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Role role, CancellationToken cancellationToken = default)
        {
            _dbContext.Roles.Remove(role);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        // ========================
        // Domain-level permission management
        // ========================
        public async Task AddPermissionAsync(Role role, Permission permission, CancellationToken cancellationToken = default)
        {
            role.AddPermission(permission);
            _dbContext.Roles.Update(role);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task RemovePermissionAsync(Role role, Permission permission, CancellationToken cancellationToken = default)
        {
            var existing = role.Permissions.FirstOrDefault(p => p.PermissionId == permission.Id);
            if (existing != null)
            {
                // EF Core tracks RolePermission because it's mapped in DbContext
                _dbContext.RolePermissions.Remove(existing);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
