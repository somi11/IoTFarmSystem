using IoTFarmSystem.DeviceManagement.Domain.Constants;
using IoTFarmSystem.DeviceManagement.Domain.Entites;

namespace IoTFarmSystem.DeviceManagement.Domain.Aggregates
{
    public class Device
    {
        public Guid Id { get; private set; }
        public string DeviceName { get; private set; }
        public string SerialNumber { get; private set; }
        public Guid TenantId { get; private set; } // Links to Tenant in UserManagement
        public bool IsActive { get; private set; }
        // Azure IoT Hub details
        public string AzureIotDeviceId { get; private set; }
        public string ConnectionString { get; private set; }

        // Device info
        public string DeviceType { get; private set; } // "LED", "UV", "Infrared", "FullSpectrum"
        public string Status { get; private set; }     // "Registered", "Online", "Offline", etc.
        public string? Location { get; private set; }
        public string? Description { get; private set; }

        // Specifications
        public int MaxIntensity { get; private set; }
        public int PowerRatingWatts { get; private set; }
        public string LightSpectrum { get; private set; }
        public string Manufacturer { get; private set; }
        public string ModelNumber { get; private set; }
        public string FirmwareVersion { get; private set; }

        // Timestamps
        public DateTime RegisteredAt { get; private set; }
        public DateTime? LastConnectedAt { get; private set; }
        public DateTime? LastMaintenanceAt { get; private set; }

        // Audit fields
        public Guid? ActivatedBy { get; private set; }
        public DateTime? ActivatedAt { get; private set; }
        public Guid? DeactivatedBy { get; private set; }
        public DateTime? DeactivatedAt { get; private set; }

        // Maintenance records
        private readonly List<MaintenanceRecord> _maintenanceRecords = new();
        public IReadOnlyCollection<MaintenanceRecord> MaintenanceRecords => _maintenanceRecords.AsReadOnly();

        private Device() { } // EF Core

        public Device(
            Guid id,
            string deviceName,
            string serialNumber,
            Guid tenantId,
            string azureIotDeviceId,
            string connectionString,
            string deviceType,
            int maxIntensity,
            int powerRatingWatts,
            string lightSpectrum,
            string manufacturer,
            string modelNumber,
            string firmwareVersion,
            string? location = null,
            string? description = null)
        {
            if (id == Guid.Empty) throw new ArgumentException("Device Id cannot be empty", nameof(id));
            if (string.IsNullOrWhiteSpace(deviceName)) throw new ArgumentException("Device name is required", nameof(deviceName));
            if (string.IsNullOrWhiteSpace(serialNumber)) throw new ArgumentException("Serial number is required", nameof(serialNumber));
            if (tenantId == Guid.Empty) throw new ArgumentException("Tenant Id cannot be empty", nameof(tenantId));
            if (maxIntensity <= 0 || maxIntensity > 100)
                throw new ArgumentException("Max intensity must be between 1 and 100", nameof(maxIntensity));

            Id = id;
            DeviceName = deviceName;
            SerialNumber = serialNumber;
            TenantId = tenantId;
            AzureIotDeviceId = azureIotDeviceId;
            ConnectionString = connectionString;
            DeviceType = deviceType;
            MaxIntensity = maxIntensity;
            PowerRatingWatts = powerRatingWatts;
            LightSpectrum = lightSpectrum;
            Manufacturer = manufacturer;
            ModelNumber = modelNumber;
            FirmwareVersion = firmwareVersion;
            Location = location;
            Description = description;

            Status = DeviceStatuses.Registered;
            RegisteredAt = DateTime.UtcNow;
        }

        // ========== Device Lifecycle ==========
        public void Activate(Guid userId)
        {
            if (ActivatedAt != null) return;

            ActivatedBy = userId;
            ActivatedAt = DateTime.UtcNow;
            Status = DeviceStatuses.Online;
        }

        public void Deactivate(Guid userId)
        {
            DeactivatedBy = userId;
            DeactivatedAt = DateTime.UtcNow;
            Status = DeviceStatuses.Inactive;
        }

        public void MarkAsFaulty(string reason)
        {
            Status = DeviceStatuses.Faulty;
        }

        public void MarkUnderMaintenance()
        {
            Status = DeviceStatuses.Maintenance;
        }

        // ========== Configuration ==========
        public void UpdateBasicInfo(string deviceName, string? location, string? description)
        {
            if (string.IsNullOrWhiteSpace(deviceName))
                throw new ArgumentException("Device name cannot be empty", nameof(deviceName));

            DeviceName = deviceName;
            Location = location;
            Description = description;
        }

        public void UpdateSpecifications(
            int maxIntensity,
            int powerRatingWatts,
            string lightSpectrum,
            string manufacturer,
            string modelNumber)
        {
            if (maxIntensity <= 0 || maxIntensity > 100)
                throw new ArgumentException("Max intensity must be between 1 and 100", nameof(maxIntensity));

            MaxIntensity = maxIntensity;
            PowerRatingWatts = powerRatingWatts;
            LightSpectrum = lightSpectrum;
            Manufacturer = manufacturer;
            ModelNumber = modelNumber;
        }

        public void UpdateFirmware(string newVersion)
        {
            if (string.IsNullOrWhiteSpace(newVersion))
                throw new ArgumentException("Firmware version cannot be empty", nameof(newVersion));

            FirmwareVersion = newVersion;
        }

        // ========== Maintenance ==========
        public void AddMaintenanceRecord(
            string performedBy,
            string maintenanceType,
            string description,
            string? notes = null)
        {
            var record = new MaintenanceRecord(
                Guid.NewGuid(),
                Id,
                maintenanceType,
                description,
                performedBy,
                DateTime.UtcNow,
                notes);

            _maintenanceRecords.Add(record);
            LastMaintenanceAt = DateTime.UtcNow;
        }

        public bool RequiresMaintenance(int daysSinceLastMaintenance = 90)
        {
            if (!LastMaintenanceAt.HasValue)
                return (DateTime.UtcNow - RegisteredAt).TotalDays > daysSinceLastMaintenance;

            return (DateTime.UtcNow - LastMaintenanceAt.Value).TotalDays > daysSinceLastMaintenance;
        }
    }
}
