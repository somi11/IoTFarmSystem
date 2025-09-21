using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.SharedKernel.Security
{
    public static class SystemRoles
    {
        public const string SYSTEM_ADMIN = "SystemAdmin";
        public const string TENANT_OWNER = "TenantOwner";
        public const string TENANT_ADMIN = "TenantAdmin";
        public const string FARM_MANAGER = "FarmManager";
        public const string TECHNICIAN = "Technician";
        public const string OPERATOR = "Operator";
        public const string VIEWER = "Viewer";
    }
}
