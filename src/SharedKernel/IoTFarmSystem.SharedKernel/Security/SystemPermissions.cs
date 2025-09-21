using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.SharedKernel.Security
{
    public static class SystemPermissions
    {
        // User
        public const string USERS_READ = "users:read";
        public const string USERS_CREATE = "users:create";
        public const string USERS_UPDATE = "users:update";
        public const string USERS_DELETE = "users:delete";

        // Device
        public const string DEVICES_READ = "devices:read";
        public const string DEVICES_CREATE = "devices:create";
        public const string DEVICES_UPDATE = "devices:update";
        public const string DEVICES_DELETE = "devices:delete";
        public const string DEVICES_MONITOR = "devices:monitor";

        // Lighting
        public const string DEVICES_POWER = "devices:power";
        public const string DEVICES_INTENSITY = "devices:intensity";
        public const string DEVICES_SCHEDULE = "devices:schedule";
    }
}
