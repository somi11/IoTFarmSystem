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
            // 1. System Admins always succeed
            if (_currentUser.IsSystemAdmin())
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // 2. Must have the permission itself
            if (!_currentUser.HasPermission(requirement.Permission))
                return Task.CompletedTask;

            // 3. Must belong to a tenant
            if (_currentUser.TenantId.HasValue)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
