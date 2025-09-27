
using IoTFarmSystem.Api.Authorization.Permission;
using IoTFarmSystem.Api.Authorization.Role;
using IoTFarmSystem.Api.Authorization.TenantOwnership;
using IoTFarmSystem.SharedKernel.Security;
using Microsoft.AspNetCore.Authorization;

namespace IoTFarmSystem.Api.Authorization.Polices
{
    public static class AuthorizationPolicies
    {
        public static void ConfigureAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                // -------------------------------------------------
                // 1. System Admin (role-based)
                // -------------------------------------------------
                options.AddPolicy("SystemAdminOnly", policy =>
                    policy.Requirements.Add(new RoleRequirement(SystemRoles.SYSTEM_ADMIN)));

                // -------------------------------------------------
                // 2. User Management
                // -------------------------------------------------
                options.AddPolicy(SystemPermissions.USERS_READ, p =>
                    p.Requirements.Add(new PermissionRequirement(SystemPermissions.USERS_READ)));

                options.AddPolicy(SystemPermissions.USERS_CREATE, p =>
                    p.Requirements.Add(new TenantOwnershipRequirement(SystemPermissions.USERS_CREATE)));

                options.AddPolicy(SystemPermissions.USERS_UPDATE, p =>
                    p.Requirements.Add(new TenantOwnershipRequirement(SystemPermissions.USERS_UPDATE)));

                options.AddPolicy(SystemPermissions.USERS_DELETE, p =>
                    p.Requirements.Add(new TenantOwnershipRequirement(SystemPermissions.USERS_DELETE)));

                options.AddPolicy(SystemPermissions.USERS_INVITE, p =>
                    p.Requirements.Add(new TenantOwnershipRequirement(SystemPermissions.USERS_INVITE)));

                options.AddPolicy(SystemPermissions.USERS_ACTIVATE, p =>
                    p.Requirements.Add(new TenantOwnershipRequirement(SystemPermissions.USERS_ACTIVATE)));

                options.AddPolicy(SystemPermissions.USERS_DEACTIVATE, p =>
                    p.Requirements.Add(new TenantOwnershipRequirement(SystemPermissions.USERS_DEACTIVATE)));

                // -------------------------------------------------
                // 3. Tenant Management
                // -------------------------------------------------
                options.AddPolicy(SystemPermissions.TENANTS_READ, p =>
                    p.Requirements.Add(new PermissionRequirement(SystemPermissions.TENANTS_READ)));

                options.AddPolicy(SystemPermissions.TENANTS_CREATE, p =>
                    p.Requirements.Add(new PermissionRequirement(SystemPermissions.TENANTS_CREATE)));

                options.AddPolicy(SystemPermissions.TENANTS_UPDATE, p =>
                    p.Requirements.Add(new TenantOwnershipRequirement(SystemPermissions.TENANTS_UPDATE)));

                options.AddPolicy(SystemPermissions.TENANTS_DELETE, p =>
                    p.Requirements.Add(new PermissionRequirement(SystemPermissions.TENANTS_DELETE)));

                options.AddPolicy(SystemPermissions.TENANTS_CONFIGURE, p =>
                    p.Requirements.Add(new TenantOwnershipRequirement(SystemPermissions.TENANTS_CONFIGURE)));

                // -------------------------------------------------
                // 4. Role & Permission Management
                // -------------------------------------------------
                options.AddPolicy(SystemPermissions.ROLES_READ, p =>
                    p.Requirements.Add(new PermissionRequirement(SystemPermissions.ROLES_READ)));

                options.AddPolicy(SystemPermissions.ROLES_CREATE, p =>
                    p.Requirements.Add(new PermissionRequirement(SystemPermissions.ROLES_CREATE)));

                options.AddPolicy(SystemPermissions.ROLES_UPDATE, p =>
                    p.Requirements.Add(new PermissionRequirement(SystemPermissions.ROLES_UPDATE)));

                options.AddPolicy(SystemPermissions.ROLES_DELETE, p =>
                    p.Requirements.Add(new PermissionRequirement(SystemPermissions.ROLES_DELETE)));

                options.AddPolicy(SystemPermissions.ROLES_ASSIGN, p =>
                    p.Requirements.Add(new TenantOwnershipRequirement(SystemPermissions.ROLES_ASSIGN)));

                options.AddPolicy(SystemPermissions.PERMISSIONS_READ, p =>
                    p.Requirements.Add(new PermissionRequirement(SystemPermissions.PERMISSIONS_READ)));

                options.AddPolicy(SystemPermissions.PERMISSIONS_ASSIGN, p =>
                    p.Requirements.Add(new TenantOwnershipRequirement(SystemPermissions.PERMISSIONS_ASSIGN)));

                options.AddPolicy(SystemPermissions.PERMISSIONS_REVOKE, p =>
                    p.Requirements.Add(new TenantOwnershipRequirement(SystemPermissions.PERMISSIONS_REVOKE)));

                // -------------------------------------------------
                // 5. Device Management
                // -------------------------------------------------
                options.AddPolicy(SystemPermissions.DEVICES_READ, p =>
                    p.Requirements.Add(new PermissionRequirement(SystemPermissions.DEVICES_READ)));

                options.AddPolicy(SystemPermissions.DEVICES_CREATE, p =>
                    p.Requirements.Add(new TenantOwnershipRequirement(SystemPermissions.DEVICES_CREATE)));

                options.AddPolicy(SystemPermissions.DEVICES_UPDATE, p =>
                    p.Requirements.Add(new TenantOwnershipRequirement(SystemPermissions.DEVICES_UPDATE)));

                options.AddPolicy(SystemPermissions.DEVICES_DELETE, p =>
                    p.Requirements.Add(new TenantOwnershipRequirement(SystemPermissions.DEVICES_DELETE)));

                options.AddPolicy(SystemPermissions.DEVICES_MONITOR, p =>
                    p.Requirements.Add(new PermissionRequirement(SystemPermissions.DEVICES_MONITOR)));

                options.AddPolicy(SystemPermissions.DEVICES_CONFIGURE, p =>
                    p.Requirements.Add(new TenantOwnershipRequirement(SystemPermissions.DEVICES_CONFIGURE)));

                options.AddPolicy(SystemPermissions.DEVICES_RESET, p =>
                    p.Requirements.Add(new TenantOwnershipRequirement(SystemPermissions.DEVICES_RESET)));

                options.AddPolicy(SystemPermissions.DEVICES_MAINTENANCE, p =>
                    p.Requirements.Add(new TenantOwnershipRequirement(SystemPermissions.DEVICES_MAINTENANCE)));

                // Device Control
                options.AddPolicy(SystemPermissions.DEVICES_POWER, p =>
                    p.Requirements.Add(new TenantOwnershipRequirement(SystemPermissions.DEVICES_POWER)));

                options.AddPolicy(SystemPermissions.DEVICES_INTENSITY, p =>
                    p.Requirements.Add(new TenantOwnershipRequirement(SystemPermissions.DEVICES_INTENSITY)));

                options.AddPolicy(SystemPermissions.DEVICES_SCHEDULE, p =>
                    p.Requirements.Add(new TenantOwnershipRequirement(SystemPermissions.DEVICES_SCHEDULE)));

                options.AddPolicy(SystemPermissions.DEVICES_EMERGENCY_STOP, p =>
                    p.Requirements.Add(new PermissionRequirement(SystemPermissions.DEVICES_EMERGENCY_STOP)));

                // -------------------------------------------------
                // 6. Analytics & Reporting
                // -------------------------------------------------
                options.AddPolicy(SystemPermissions.ANALYTICS_READ, p =>
                    p.Requirements.Add(new PermissionRequirement(SystemPermissions.ANALYTICS_READ)));

                options.AddPolicy(SystemPermissions.REPORTS_READ, p =>
                    p.Requirements.Add(new PermissionRequirement(SystemPermissions.REPORTS_READ)));

                options.AddPolicy(SystemPermissions.REPORTS_CREATE, p =>
                    p.Requirements.Add(new PermissionRequirement(SystemPermissions.REPORTS_CREATE)));

                options.AddPolicy(SystemPermissions.REPORTS_EXPORT, p =>
                    p.Requirements.Add(new PermissionRequirement(SystemPermissions.REPORTS_EXPORT)));

                // -------------------------------------------------
                // 7. System Configuration
                // -------------------------------------------------
                options.AddPolicy(SystemPermissions.SYSTEM_SETTINGS_READ, p =>
                    p.Requirements.Add(new PermissionRequirement(SystemPermissions.SYSTEM_SETTINGS_READ)));

                options.AddPolicy(SystemPermissions.SYSTEM_SETTINGS_UPDATE, p =>
                    p.Requirements.Add(new PermissionRequirement(SystemPermissions.SYSTEM_SETTINGS_UPDATE)));

                options.AddPolicy(SystemPermissions.SYSTEM_BACKUP, p =>
                    p.Requirements.Add(new PermissionRequirement(SystemPermissions.SYSTEM_BACKUP)));

                options.AddPolicy(SystemPermissions.SYSTEM_RESTORE, p =>
                    p.Requirements.Add(new PermissionRequirement(SystemPermissions.SYSTEM_RESTORE)));

                // -------------------------------------------------
                // 8. Audit & Logs
                // -------------------------------------------------
                options.AddPolicy(SystemPermissions.AUDIT_READ, p =>
                    p.Requirements.Add(new PermissionRequirement(SystemPermissions.AUDIT_READ)));

                options.AddPolicy(SystemPermissions.LOGS_READ, p =>
                    p.Requirements.Add(new PermissionRequirement(SystemPermissions.LOGS_READ)));

                options.AddPolicy(SystemPermissions.LOGS_EXPORT, p =>
                    p.Requirements.Add(new PermissionRequirement(SystemPermissions.LOGS_EXPORT)));

                // -------------------------------------------------
                // 9. Composite Policies (special cases)
                // -------------------------------------------------
                options.AddPolicy("TenantOwnerOrSystemAdmin", policy =>
                    policy.RequireAssertion(context =>
                    {
                        var user = context.User;
                        return user.HasClaim("role", SystemRoles.SYSTEM_ADMIN) ||
                               user.HasClaim("role", SystemRoles.TENANT_OWNER);
                    }));

                options.AddPolicy("CanManageTenantUsers", policy =>
                    policy.RequireAssertion(context =>
                    {
                        var user = context.User;
                        return user.HasClaim("permission", SystemPermissions.USERS_CREATE) &&
                               (user.HasClaim("role", SystemRoles.TENANT_OWNER) ||
                                user.HasClaim("role", SystemRoles.TENANT_ADMIN));
                    }));
            });

            // -------------------------------------------------
            // Register authorization handlers
            // -------------------------------------------------
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();
            services.AddScoped<IAuthorizationHandler, TenantOwnershipHandler>();
            services.AddScoped<IAuthorizationHandler, RoleRequirementHandler>();
        }
    }
}
