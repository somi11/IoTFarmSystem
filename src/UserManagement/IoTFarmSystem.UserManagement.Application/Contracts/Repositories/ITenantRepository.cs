using IoTFarmSystem.UserManagement.Application.DTOs;

public interface ITenantRepository
{
    // Basic tenant queries
    Task<Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TenantDto?> GetByIdQueryAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TenantDto?> GetByNameQueryAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Tenant>> GetAllWithFarmersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TenantDto>> GetAllWithFarmersQueryAsync(CancellationToken cancellationToken = default);
    // Tenant queries by farmer
    Task<Tenant?> GetByFarmerIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Farmer>> GetFarmersByRoleAsync(Guid tenantId, string roleName, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Farmer>> GetFarmersByPermissionAsync(Guid tenantId, string permissionName, CancellationToken cancellationToken = default);

    // CRUD operations
    Task AddAsync(Tenant tenant, CancellationToken cancellationToken = default);
    Task UpdateAsync(Tenant tenant, CancellationToken cancellationToken = default);
    Task DeleteAsync(Tenant tenant, CancellationToken cancellationToken = default);

    // Optional: domain-level farmer operations
    Task AddFarmerAsync(Tenant tenant, Farmer farmer, CancellationToken cancellationToken = default);
    Task RemoveFarmerAsync(Tenant tenant, Farmer farmer, CancellationToken cancellationToken = default);
}
