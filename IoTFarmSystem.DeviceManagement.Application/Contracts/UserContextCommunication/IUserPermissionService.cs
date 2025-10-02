using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.DeviceManagement.Application.Contracts.UserContextCommunication
{
    /// <summary>
    /// Cross-context contract to validate user permissions and roles.
    /// Implemented in UserManagement.
    /// </summary>
    public interface IUserPermissionService
    {
        /// <summary>
        /// Check if user belongs to a tenant.
        /// </summary>
        Task<bool> UserBelongsToTenantAsync(Guid userId, Guid tenantId);

        /// <summary>
        /// Check if user has a specific permission.
        /// </summary>
        Task<bool> UserHasPermissionAsync(Guid userId, string permission);

        /// <summary>
        /// Get all effective permissions for a user.
        /// </summary>
        Task<List<string>> GetUserPermissionsAsync(Guid userId);

        /// <summary>
        /// Check if user has a specific role.
        /// </summary>
        Task<bool> UserHasRoleAsync(Guid userId, string roleName);
    }
}
