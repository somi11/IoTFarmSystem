using IoTFarmSystem.UserManagement.Application.Contracts.Authorizatioon;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace IoTFarmSystem.Api.Authorization.PermissionRequirement
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IAuthorizationClaimsProvider _claimsProvider;

        public PermissionHandler(IAuthorizationClaimsProvider claimsProvider)
        {
            _claimsProvider = claimsProvider;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            // Check JWT claims directly
            if (context.User.HasClaim(c => c.Type == "permission" && c.Value == requirement.Permission))
            {
                context.Succeed(requirement);
                return;
            }

            // Fallback: load claims from provider (DB/service)
            var identityId = context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(identityId)) return;

            var perms = await _claimsProvider.GetPermissionsAsync(identityId);
            if (perms.Contains(requirement.Permission))
                context.Succeed(requirement);
        }
    }
}
 