using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.SharedKernel.Security
{
    public static class SystemPermissions
    {
        // User Management
        public const string USERS_READ = "users:read";
        public const string USERS_CREATE = "users:create";
        public const string USERS_UPDATE = "users:update";
        public const string USERS_DELETE = "users:delete";
        public const string USERS_INVITE = "users:invite";
        public const string USERS_ACTIVATE = "users:activate";
        public const string USERS_DEACTIVATE = "users:deactivate";

        // Tenant Management
        public const string TENANTS_READ = "tenants:read";
        public const string TENANTS_CREATE = "tenants:create";
        public const string TENANTS_UPDATE = "tenants:update";
        public const string TENANTS_DELETE = "tenants:delete";
        public const string TENANTS_CONFIGURE = "tenants:configure";

        // Role Management
        public const string ROLES_READ = "roles:read";
        public const string ROLES_CREATE = "roles:create";
        public const string ROLES_UPDATE = "roles:update";
        public const string ROLES_DELETE = "roles:delete";
        public const string ROLES_ASSIGN = "roles:assign";

        // Permission Management
        public const string PERMISSIONS_READ = "permissions:read";
        public const string PERMISSIONS_ASSIGN = "permissions:assign";
        public const string PERMISSIONS_REVOKE = "permissions:revoke";

        // Device Management
        public const string DEVICES_READ = "devices:read";
        public const string DEVICES_CREATE = "devices:create";
        public const string DEVICES_UPDATE = "devices:update";
        public const string DEVICES_DELETE = "devices:delete";
        public const string DEVICES_MONITOR = "devices:monitor";
        public const string DEVICES_CONFIGURE = "devices:configure";
        public const string DEVICES_RESET = "devices:reset";
        public const string DEVICES_MAINTENANCE = "devices:maintenance";

        // Device Control (Lighting)
        public const string DEVICES_POWER = "devices:power";
        public const string DEVICES_INTENSITY = "devices:intensity";
        public const string DEVICES_SCHEDULE = "devices:schedule";
        public const string DEVICES_EMERGENCY_STOP = "devices:emergency_stop";

        // Analytics & Reporting
        public const string ANALYTICS_READ = "analytics:read";
        public const string REPORTS_READ = "reports:read";
        public const string REPORTS_CREATE = "reports:create";
        public const string REPORTS_EXPORT = "reports:export";

        // System Configuration
        public const string SYSTEM_SETTINGS_READ = "system:settings:read";
        public const string SYSTEM_SETTINGS_UPDATE = "system:settings:update";
        public const string SYSTEM_BACKUP = "system:backup";
        public const string SYSTEM_RESTORE = "system:restore";

        // Audit & Logs
        public const string AUDIT_READ = "audit:read";
        public const string LOGS_READ = "logs:read";
        public const string LOGS_EXPORT = "logs:export";

    }
}
