using Microsoft.AspNetCore.Authorization;

namespace IoTFarmSystem.Api.Authorization.TenantOwnership
{
    // Tenant ownership requirement
    public class TenantOwnershipRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }
        public TenantOwnershipRequirement(string permission) => Permission = permission;
    }
}
