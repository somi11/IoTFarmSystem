namespace IoTFarmSystem.UserManagement.Application.Contracts.Authorizatioon
{
    public interface IAuthorizationClaimsProvider
    {
        Task<IReadOnlyCollection<string>> GetRolesAsync(string identityUserId, CancellationToken ct = default);
        Task<IReadOnlyCollection<string>> GetPermissionsAsync(string identityUserId, CancellationToken ct = default);

        Task<(Guid? FarmerId, Guid? TenantId, string? Email)> GetDomainIdentifiersAsync(
            string identityUserId,
            CancellationToken ct = default);
        Task<IReadOnlyCollection<string>> GetEffectivePermissionsAsync(
            string identityUserId,
            CancellationToken ct = default);

    }
}
