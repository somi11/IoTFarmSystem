using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.SharedKernel.Abstractions
{
    public interface ICurrentUserService
    {
        // Basic user info
        string UserId { get; }
        string Username { get; }
        string Email { get; }
        Guid? TenantId { get; }
        Guid? FarmerId { get; } // Keep for JWT compatibility, but functionally same as TenantId

        // Claims and permissions
        bool IsAuthenticated { get; }
        bool HasClaim(string type, string value);
        bool HasPermission(string permission);
        bool HasRole(string role);
        IEnumerable<string> GetRoles();
        IEnumerable<string> GetPermissions();

        // Tenant context (simplified)
        bool BelongsToTenant(Guid tenantId);
        bool IsSystemAdmin();
        bool IsTenantOwner();
        bool IsTenantAdmin();

        // Resource access validation
        bool CanAccessResource(string resourceType, Guid resourceId);
        ClaimsPrincipal GetClaimsPrincipal();

        // Backward compatibility
        string? IdentityUserId { get; }

    }
}
