using System.Collections.Immutable;

namespace IoTFarmSystem.SharedKernel.Security
{
    public static class RolePermissionsMap
    {
        public static readonly IReadOnlyDictionary<string, string[]> Map = new Dictionary<string, string[]>
        {
            [SystemRoles.SYSTEM_ADMIN] = new[]
            {
                // User Management
                SystemPermissions.USERS_READ, SystemPermissions.USERS_CREATE, SystemPermissions.USERS_UPDATE,
                SystemPermissions.USERS_DELETE, SystemPermissions.USERS_INVITE, SystemPermissions.USERS_ACTIVATE,
                SystemPermissions.USERS_DEACTIVATE,

                // Tenant Management
                SystemPermissions.TENANTS_READ, SystemPermissions.TENANTS_CREATE, SystemPermissions.TENANTS_UPDATE,
                SystemPermissions.TENANTS_DELETE, SystemPermissions.TENANTS_CONFIGURE,

                // Role & Permission Management
                SystemPermissions.ROLES_READ, SystemPermissions.ROLES_CREATE, SystemPermissions.ROLES_UPDATE,
                SystemPermissions.ROLES_DELETE, SystemPermissions.ROLES_ASSIGN,

                SystemPermissions.PERMISSIONS_READ, SystemPermissions.PERMISSIONS_ASSIGN, SystemPermissions.PERMISSIONS_REVOKE,

                // Device Management
                SystemPermissions.DEVICES_READ, SystemPermissions.DEVICES_CREATE, SystemPermissions.DEVICES_UPDATE,
                SystemPermissions.DEVICES_DELETE, SystemPermissions.DEVICES_MONITOR, SystemPermissions.DEVICES_CONFIGURE,
                SystemPermissions.DEVICES_RESET, SystemPermissions.DEVICES_MAINTENANCE,

                // Device Control
                SystemPermissions.DEVICES_POWER, SystemPermissions.DEVICES_INTENSITY, SystemPermissions.DEVICES_SCHEDULE,
                SystemPermissions.DEVICES_EMERGENCY_STOP,

                // Analytics & Reporting
                SystemPermissions.ANALYTICS_READ, SystemPermissions.REPORTS_READ, SystemPermissions.REPORTS_CREATE,
                SystemPermissions.REPORTS_EXPORT,

                // System
                SystemPermissions.SYSTEM_SETTINGS_READ, SystemPermissions.SYSTEM_SETTINGS_UPDATE,
                SystemPermissions.SYSTEM_BACKUP, SystemPermissions.SYSTEM_RESTORE,

                // Audit & Logs
                SystemPermissions.AUDIT_READ, SystemPermissions.LOGS_READ, SystemPermissions.LOGS_EXPORT
            },

            [SystemRoles.TENANT_OWNER] = new[]
            {
                // Tenant-scoped full access
                SystemPermissions.USERS_READ, SystemPermissions.USERS_CREATE, SystemPermissions.USERS_UPDATE,
                SystemPermissions.USERS_INVITE, SystemPermissions.USERS_ACTIVATE, SystemPermissions.USERS_DEACTIVATE,

                SystemPermissions.TENANTS_READ, SystemPermissions.TENANTS_UPDATE, SystemPermissions.TENANTS_CONFIGURE,

                SystemPermissions.ROLES_READ, SystemPermissions.ROLES_ASSIGN,
                SystemPermissions.PERMISSIONS_READ, SystemPermissions.PERMISSIONS_ASSIGN, SystemPermissions.PERMISSIONS_REVOKE,

                SystemPermissions.DEVICES_READ, SystemPermissions.DEVICES_CREATE, SystemPermissions.DEVICES_UPDATE,
                SystemPermissions.DEVICES_DELETE, SystemPermissions.DEVICES_MONITOR, SystemPermissions.DEVICES_CONFIGURE,
                SystemPermissions.DEVICES_RESET,

                SystemPermissions.DEVICES_POWER, SystemPermissions.DEVICES_INTENSITY, SystemPermissions.DEVICES_SCHEDULE,
                SystemPermissions.DEVICES_EMERGENCY_STOP,

                SystemPermissions.ANALYTICS_READ, SystemPermissions.REPORTS_READ, SystemPermissions.REPORTS_CREATE,
                SystemPermissions.REPORTS_EXPORT,

                SystemPermissions.AUDIT_READ, SystemPermissions.LOGS_READ
            },

            [SystemRoles.TENANT_ADMIN] = new[]
            {
                SystemPermissions.USERS_READ, SystemPermissions.USERS_CREATE, SystemPermissions.USERS_UPDATE,
                SystemPermissions.USERS_INVITE, SystemPermissions.USERS_ACTIVATE,

                SystemPermissions.TENANTS_READ, SystemPermissions.TENANTS_UPDATE,

                SystemPermissions.ROLES_READ, SystemPermissions.ROLES_ASSIGN,
                SystemPermissions.PERMISSIONS_READ, SystemPermissions.PERMISSIONS_ASSIGN,

                SystemPermissions.DEVICES_READ, SystemPermissions.DEVICES_CREATE, SystemPermissions.DEVICES_UPDATE,
                SystemPermissions.DEVICES_MONITOR, SystemPermissions.DEVICES_CONFIGURE,

                SystemPermissions.DEVICES_POWER, SystemPermissions.DEVICES_INTENSITY, SystemPermissions.DEVICES_SCHEDULE,
                SystemPermissions.DEVICES_EMERGENCY_STOP,

                SystemPermissions.ANALYTICS_READ, SystemPermissions.REPORTS_READ, SystemPermissions.REPORTS_CREATE,
                SystemPermissions.LOGS_READ
            },

            [SystemRoles.FARM_MANAGER] = new[]
            {
                // Now equivalent to Tenant Manager role
                SystemPermissions.USERS_READ,

                SystemPermissions.DEVICES_READ, SystemPermissions.DEVICES_UPDATE, SystemPermissions.DEVICES_MONITOR,
                SystemPermissions.DEVICES_POWER, SystemPermissions.DEVICES_INTENSITY, SystemPermissions.DEVICES_SCHEDULE,
                SystemPermissions.DEVICES_EMERGENCY_STOP,

                SystemPermissions.TENANTS_READ, SystemPermissions.TENANTS_UPDATE,

                SystemPermissions.ANALYTICS_READ, SystemPermissions.REPORTS_READ, SystemPermissions.REPORTS_CREATE
            },

            [SystemRoles.TECHNICIAN] = new[]
            {
                SystemPermissions.DEVICES_READ, SystemPermissions.DEVICES_UPDATE, SystemPermissions.DEVICES_MONITOR,
                SystemPermissions.DEVICES_CONFIGURE, SystemPermissions.DEVICES_RESET, SystemPermissions.DEVICES_MAINTENANCE,
                SystemPermissions.DEVICES_SCHEDULE,

                SystemPermissions.TENANTS_READ,

                SystemPermissions.ANALYTICS_READ, SystemPermissions.REPORTS_READ
            },

            [SystemRoles.OPERATOR] = new[]
            {
                SystemPermissions.DEVICES_READ, SystemPermissions.DEVICES_MONITOR,
                SystemPermissions.DEVICES_POWER, SystemPermissions.DEVICES_INTENSITY,

                SystemPermissions.TENANTS_READ,

                SystemPermissions.ANALYTICS_READ
            },

            [SystemRoles.VIEWER] = new[]
            {
                SystemPermissions.DEVICES_READ, SystemPermissions.DEVICES_MONITOR,
                SystemPermissions.TENANTS_READ,
                SystemPermissions.ANALYTICS_READ
            }
        }.ToImmutableDictionary();
    }
}
