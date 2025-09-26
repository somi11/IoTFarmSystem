namespace IoTFarmSystem.UserManagement.Application.Contracts.Authorizatioon
{
    public interface IAuthorizationClaimsProvider
    {
        Task<IReadOnlyCollection<string>> GetPermissionsAsync(string identityUserId, CancellationToken ct = default);
        Task<IReadOnlyCollection<string>> GetRolesAsync(string identityUserId, CancellationToken ct = default);
   
   
    }
}
