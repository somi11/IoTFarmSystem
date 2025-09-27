﻿// Enhanced CurrentUserService.cs
using IoTFarmSystem.SharedKernel.Abstractions;
using IoTFarmSystem.SharedKernel.Security;
using System.Security.Claims;

namespace IoTFarmSystem.Api.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Basic user info
        public string UserId => User?.FindFirstValue(ClaimTypes.NameIdentifier)
                               ?? User?.FindFirstValue("sub")
                               ?? string.Empty;

        public string Username => User?.FindFirstValue(ClaimTypes.Name)
                                 ?? User?.FindFirstValue("preferred_username")
                                 ?? Email;

        public string Email => User?.FindFirstValue(ClaimTypes.Email)
                              ?? User?.FindFirstValue("email")
                              ?? string.Empty;

        public Guid? FarmerId => TryParseGuidClaim("farmerId");
        public Guid? TenantId => TryParseGuidClaim("tenantId");

        // Backward compatibility
        public string? IdentityUserId => UserId;

        // Authentication and authorization
        public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

        public bool HasClaim(string type, string value) =>
            User?.HasClaim(c => c.Type == type && c.Value == value) ?? false;

        public bool HasPermission(string permission) =>
            User?.FindAll("permission").Any(c => c.Value == permission) ?? false;

        public bool HasRole(string role) =>
            User?.FindAll("role").Any(c => c.Value == role) ?? false;

        public IEnumerable<string> GetRoles() =>
            User?.FindAll("role").Select(c => c.Value) ?? Enumerable.Empty<string>();

        public IEnumerable<string> GetPermissions() =>
            User?.FindAll("permission").Select(c => c.Value) ?? Enumerable.Empty<string>();

        // Tenant context
        public bool BelongsToTenant(Guid tenantId)
        {
            if (IsSystemAdmin()) return true;
            return TenantId.HasValue && TenantId.Value == tenantId;
        }

       

        public bool IsSystemAdmin() => HasRole(SystemRoles.SYSTEM_ADMIN);
        public bool IsTenantOwner() => HasRole(SystemRoles.TENANT_OWNER);
        public bool IsTenantAdmin() => HasRole(SystemRoles.TENANT_ADMIN);

        public bool CanAccessResource(string resourceType, Guid resourceId)
        {
            if (IsSystemAdmin()) return true;

            return TenantId.HasValue;
        }

        public ClaimsPrincipal GetClaimsPrincipal() => User ?? new ClaimsPrincipal();

        private Guid? TryParseGuidClaim(string claimType)
        {
            var value = User?.FindFirstValue(claimType);
            return Guid.TryParse(value, out var guid) ? guid : null;
        }
    }
}