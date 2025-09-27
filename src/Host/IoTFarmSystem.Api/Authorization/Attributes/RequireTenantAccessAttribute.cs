using Microsoft.AspNetCore.Authorization;

namespace IoTFarmSystem.Api.Authorization.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class RequireTenantAccessAttribute : AuthorizeAttribute
    {
        public RequireTenantAccessAttribute(string permission)
        {
            Policy = $"RequireTenantAccess_{permission.Replace(":", "_")}";
        }
    }
}
