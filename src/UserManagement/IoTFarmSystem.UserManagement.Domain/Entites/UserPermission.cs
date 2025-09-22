using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Domain.Entites
{
    public record UserPermission
    {
        public Guid UserId { get; init; }
        public Guid PermissionId { get; init; }
        public string PermissionName { get; init; }
        
        // Navigation property (not in constructor)
        public Permission? Permission { get; init; }

        // Parameterless constructor for EF Core
        private UserPermission() { }

        // Public constructor for your own use
        public UserPermission(Guid userId, Guid permissionId, string permissionName , Permission permission)
        {
            UserId = userId;
            PermissionId = permissionId;
            PermissionName = permissionName;
            Permission = permission;
        }
    }
}
