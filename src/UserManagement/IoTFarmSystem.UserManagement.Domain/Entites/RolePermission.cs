using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Domain.Entites
{
    public class RolePermission
    {
        public Guid RoleId { get; private set; }
        public Guid PermissionId { get; private set; }

        // Navigation (optional, depending on your mapping)
        public Role Role { get; private set; }
        public Permission Permission { get; private set; }

        private RolePermission() { } // EF Core

        public RolePermission(Guid roleId, Guid permissionId)
        {
            if (roleId == Guid.Empty)
                throw new ArgumentException("Role Id cannot be empty.", nameof(roleId));
            if (permissionId == Guid.Empty)
                throw new ArgumentException("Permission Id cannot be empty.", nameof(permissionId));

            RoleId = roleId;
            PermissionId = permissionId;
        }
    }

}
