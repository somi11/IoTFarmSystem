using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IoTFarmSystem.UserManagement.Domain.Entites;

namespace IoTFarmSystem.UserManagement.Application.Contracts.Repositories
{
    public interface IRoleRepository
    {
        // Basic role queries
        Task<Role?> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default);
        Task<Role?> GetByNameAsync(string roleName, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default);

        // CRUD
        Task AddAsync(Role role, CancellationToken cancellationToken = default);
        Task UpdateAsync(Role role, CancellationToken cancellationToken = default);
        Task DeleteAsync(Role role, CancellationToken cancellationToken = default);

        // Domain-level permission management
        Task AddPermissionAsync(Role role, Permission permission, CancellationToken cancellationToken = default);
        Task RemovePermissionAsync(Role role, Permission permission, CancellationToken cancellationToken = default);
    }
}
