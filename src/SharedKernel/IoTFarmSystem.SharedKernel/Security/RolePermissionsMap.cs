using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.SharedKernel.Security
{
    public static class RolePermissionsMap
    {
        // maps role -> list of permission strings
        public static readonly IReadOnlyDictionary<string, string[]> Map = new Dictionary<string, string[]>
        {
            [SystemRoles.SYSTEM_ADMIN] = new[]
            {
            SystemPermissions.USERS_READ, SystemPermissions.USERS_CREATE, SystemPermissions.USERS_UPDATE, SystemPermissions.USERS_DELETE,
            SystemPermissions.DEVICES_READ, SystemPermissions.DEVICES_CREATE, SystemPermissions.DEVICES_UPDATE, SystemPermissions.DEVICES_DELETE,
            SystemPermissions.DEVICES_POWER, SystemPermissions.DEVICES_INTENSITY, SystemPermissions.DEVICES_SCHEDULE, SystemPermissions.DEVICES_MONITOR
        },
            [SystemRoles.TENANT_OWNER] = new[]
            {
            SystemPermissions.USERS_READ, SystemPermissions.USERS_CREATE,
            SystemPermissions.DEVICES_READ, SystemPermissions.DEVICES_CREATE,
            SystemPermissions.DEVICES_POWER, SystemPermissions.DEVICES_INTENSITY, SystemPermissions.DEVICES_SCHEDULE, SystemPermissions.DEVICES_MONITOR
        },
            [SystemRoles.FARM_MANAGER] = new[]
            {
            SystemPermissions.DEVICES_READ, SystemPermissions.DEVICES_POWER, SystemPermissions.DEVICES_INTENSITY, SystemPermissions.DEVICES_SCHEDULE, SystemPermissions.DEVICES_MONITOR
        },
            [SystemRoles.TECHNICIAN] = new[]
            {
            SystemPermissions.DEVICES_READ, SystemPermissions.DEVICES_UPDATE, SystemPermissions.DEVICES_MONITOR, SystemPermissions.DEVICES_SCHEDULE
        },
            [SystemRoles.OPERATOR] = new[]
            {
            SystemPermissions.DEVICES_READ, SystemPermissions.DEVICES_POWER, SystemPermissions.DEVICES_INTENSITY
        },
            [SystemRoles.VIEWER] = new[]
            {
            SystemPermissions.DEVICES_READ, SystemPermissions.DEVICES_MONITOR
        }
        }.ToImmutableDictionary();
    }
}