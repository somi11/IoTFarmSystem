using IoTFarmSystem.DeviceManagement.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.DeviceManagement.Application.Contracts.Repositories
{
    /// <summary>
    /// Repository for Device aggregate
    /// </summary>
    public interface IDeviceRepository
    {
        Task<Device?> GetByIdAsync(Guid id);
        Task<Device?> GetBySerialNumberAsync(string serialNumber);
        Task<List<Device>> GetByTenantIdAsync(Guid tenantId);
        Task<List<Device>> GetAllAsync();
        Task AddAsync(Device device);
        Task UpdateAsync(Device device);
        Task DeleteAsync(Device device);
        Task<bool> SerialNumberExistsAsync(string serialNumber);
    }
}
