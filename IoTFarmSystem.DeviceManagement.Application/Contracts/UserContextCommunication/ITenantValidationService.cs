using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.DeviceManagement.Application.Contracts.UserContextCommunication
{
    /// <summary>
    /// Cross-context contract to validate tenant-related operations.
    /// Implemented in UserManagement.
    /// </summary>
    public interface ITenantValidationService
    {
        /// <summary>
        /// Check if a tenant exists.
        /// </summary>
        Task<bool> TenantExistsAsync(Guid tenantId);

        /// <summary>
        /// Get tenant name by ID.
        /// </summary>
        Task<string?> GetTenantNameAsync(Guid tenantId);

        /// <summary>
        /// Check if tenant is active.
        /// </summary>
        Task<bool> IsTenantActiveAsync(Guid tenantId);
    }
}
