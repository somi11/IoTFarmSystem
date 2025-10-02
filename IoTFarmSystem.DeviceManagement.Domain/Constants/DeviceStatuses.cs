using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.DeviceManagement.Domain.Constants
{
    public static class DeviceStatuses
    {
        public const string Registered = "Registered";
        public const string Online = "Online";
        public const string Offline = "Offline";
        public const string Faulty = "Faulty";
        public const string Inactive = "Inactive";
        public const string Maintenance = "Maintenance";
    }
}
