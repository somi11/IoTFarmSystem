using IoTFarmSystem.SharedKernel.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace IoTFarmSystem.Api.Extensions
{
    public static class AuthorizationExtensions
    {
        public static async Task<IActionResult> ValidateTenantAccessAsync(
            this ControllerBase controller,
            ICurrentUserService currentUser,
            Guid resourceTenantId,
            Func<Task<IActionResult>> operation)
        {
            if (!currentUser.BelongsToTenant(resourceTenantId) && !currentUser.IsSystemAdmin())
            {
                return controller.Forbid();
            }

            return await operation();
        }
    }
}
