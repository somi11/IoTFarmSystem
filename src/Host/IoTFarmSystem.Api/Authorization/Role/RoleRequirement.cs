using Microsoft.AspNetCore.Authorization;

namespace IoTFarmSystem.Api.Authorization.Role;

public class RoleRequirement : IAuthorizationRequirement
{
    public string Role { get; }
    public RoleRequirement(string role) => Role = role;
}
