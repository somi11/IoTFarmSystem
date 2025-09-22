using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IoTFarmSystem.UserManagement.Domain.Entites;

namespace IoTFarmSystem.UserManagement.Application.Contracts.Repositories
{
    public interface IFarmerRepository
    {
        // Basic queries
        Task<Farmer?> GetByIdAsync(Guid farmerId, CancellationToken cancellationToken = default);
        Task<Farmer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Farmer>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
        Task<Farmer?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken = default);

        // Queries including roles/permissions
        Task<Farmer?> GetWithRolesAsync(Guid farmerId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Farmer>> GetByRoleAsync(string roleName, CancellationToken cancellationToken = default);
        Task<Farmer?> GetWithPermissionsAsync(Guid farmerId, CancellationToken cancellationToken = default);

        // CRUD operations
        Task AddAsync(Farmer farmer, CancellationToken cancellationToken = default);
        Task UpdateAsync(Farmer farmer, CancellationToken cancellationToken = default);
        Task DeleteAsync(Farmer farmer, CancellationToken cancellationToken = default);

        // Domain-level role management
        Task AssignRoleAsync(Farmer farmer, Role role, CancellationToken cancellationToken = default);
        Task RevokeRoleAsync(Farmer farmer, Role role, CancellationToken cancellationToken = default);

        // Domain-level permission management
        Task GrantPermissionAsync(Farmer farmer, Permission permission, CancellationToken cancellationToken = default);
        Task RevokePermissionAsync(Farmer farmer, Permission permission, CancellationToken cancellationToken = default);
    }
}
