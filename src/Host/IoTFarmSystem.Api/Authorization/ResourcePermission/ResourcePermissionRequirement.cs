using Microsoft.AspNetCore.Authorization;

namespace IoTFarmSystem.Api.Authorization.ResourcePermission
{
    // Resource-based requirement (for tenant-scoped operations)
    public class ResourcePermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }
        public string ResourceType { get; }

        public ResourcePermissionRequirement(string permission, string resourceType = null)
        {
            Permission = permission;
            ResourceType = resourceType;
        }
    }
}
