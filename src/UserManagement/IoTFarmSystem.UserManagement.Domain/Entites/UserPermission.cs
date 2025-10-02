using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Domain.Entites
{
    public class UserPermission
    {
        public Guid UserId { get; private set; }
        public Guid PermissionId { get; private set; }
        public string PermissionName { get; private set; }

        public Permission? Permission { get; private set; }

        private UserPermission() { } // EF

        public UserPermission(Guid userId, Guid permissionId, string permissionName, Permission? permission = null)
        {
            UserId = userId;
            PermissionId = permissionId;
            PermissionName = permissionName;
            Permission = permission; // optional (navigation only)
        }
    }
}
