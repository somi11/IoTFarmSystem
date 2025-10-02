using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.DeviceManagement.Domain.Constants
{
    public static class ConnectionEventTypes
    {
        public const string Connected = "Connected";
        public const string Disconnected = "Disconnected";
        public const string Error = "Error";
        public const string Reset = "Reset";
        public const string ConfigurationChanged = "ConfigurationChanged";
    }
}
