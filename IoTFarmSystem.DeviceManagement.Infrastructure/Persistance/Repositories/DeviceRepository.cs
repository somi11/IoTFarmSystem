using IoTFarmSystem.DeviceManagement.Application.Contracts.Repositories;
using IoTFarmSystem.DeviceManagement.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.DeviceManagement.Infrastructure.Persistance.Repositories
{

        public class DeviceRepository : IDeviceRepository
        {
            private readonly DeviceManagementDbContext _context;

            public DeviceRepository(DeviceManagementDbContext context)
            {
                _context = context;
            }

            public async Task<Device?> GetByIdAsync(Guid id)
            {
                return await _context.Devices
                    .FirstOrDefaultAsync(d => d.Id == id);
            }

            public async Task<Device?> GetBySerialNumberAsync(string serialNumber)
            {
                return await _context.Devices
                    .FirstOrDefaultAsync(d => d.SerialNumber == serialNumber);
            }

            public async Task<List<Device>> GetByTenantIdAsync(Guid tenantId)
            {
                return await _context.Devices
                    .Where(d => d.TenantId == tenantId && d.IsActive)
                    .ToListAsync();
            }

            public async Task<List<Device>> GetAllAsync()
            {
                return await _context.Devices
                    .Where(d => d.IsActive)
                    .ToListAsync();
            }

            public async Task AddAsync(Device device)
            {
                await _context.Devices.AddAsync(device);
                
            }

            public Task UpdateAsync(Device device)
            {
                _context.Devices.Update(device);
               
                return Task.CompletedTask;
            }

            public Task DeleteAsync(Device device)
            {
                _context.Devices.Remove(device);
               
                return Task.CompletedTask;
            }

            public async Task<bool> SerialNumberExistsAsync(string serialNumber)
            {
                return await _context.Devices
                    .AnyAsync(d => d.SerialNumber == serialNumber);
            }
        }
    }


