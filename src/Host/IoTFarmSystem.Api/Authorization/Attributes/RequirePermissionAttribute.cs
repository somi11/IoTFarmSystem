using Microsoft.AspNetCore.Authorization;

namespace IoTFarmSystem.Api.Authorization.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class RequirePermissionAttribute : AuthorizeAttribute
    {
        public RequirePermissionAttribute(string permission)
        {
            Policy = $"Require_{permission.Replace(":", "_")}";
        }
    }
}
