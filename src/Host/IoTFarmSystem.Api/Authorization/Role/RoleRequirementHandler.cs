using IoTFarmSystem.SharedKernel.Abstractions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace IoTFarmSystem.Api.Authorization.Role;

public class RoleRequirementHandler : AuthorizationHandler<RoleRequirement>
{
    private readonly ICurrentUserService _currentUser;

    public RoleRequirementHandler(ICurrentUserService currentUser)
    {
        _currentUser = currentUser;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RoleRequirement requirement)
    {
        // both claim types ("role" and ClaimTypes.Role)
        if (_currentUser.HasClaim("role", requirement.Role) ||
            _currentUser.HasClaim(ClaimTypes.Role, requirement.Role))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
