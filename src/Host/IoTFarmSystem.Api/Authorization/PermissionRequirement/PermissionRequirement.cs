using Microsoft.AspNetCore.Authorization;

namespace IoTFarmSystem.Api.Authorization.PermissionRequirement
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }
        public PermissionRequirement(string permission) => Permission = permission;
    }
}
