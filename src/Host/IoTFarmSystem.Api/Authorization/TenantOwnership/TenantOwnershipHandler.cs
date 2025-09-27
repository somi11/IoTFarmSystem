using IoTFarmSystem.SharedKernel.Abstractions;
using Microsoft.AspNetCore.Authorization;

namespace IoTFarmSystem.Api.Authorization.TenantOwnership
{
    public class TenantOwnershipHandler : AuthorizationHandler<TenantOwnershipRequirement>
    {
        private readonly ICurrentUserService _currentUser;

        public TenantOwnershipHandler(ICurrentUserService currentUser)
        {
            _currentUser = currentUser;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            TenantOwnershipRequirement requirement)
        {
            // Check if user has the required permission within their tenant scope
            if (_currentUser.HasPermission(requirement.Permission) && _currentUser.TenantId.HasValue)
            {
                // Additional tenant-specific validation can be added here
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
