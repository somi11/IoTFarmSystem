using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.DeviceManagement.Application.Dtos
{
    public class DeviceDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string DeviceName { get; set; } = default!;
        public string SerialNumber { get; set; } = default!;
        public string DeviceType { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string? Location { get; set; }
        public string? Description { get; set; }

        public int MaxIntensity { get; set; }
        public int PowerRatingWatts { get; set; }
        public string LightSpectrum { get; set; } = default!;
        public string Manufacturer { get; set; } = default!;
        public string ModelNumber { get; set; } = default!;
        public string FirmwareVersion { get; set; } = default!;

        public bool IsActive { get; set; }
        public bool IsPoweredOn { get; set; }
        public int CurrentIntensityLevel { get; set; }

        public DateTime RegisteredAt { get; set; }
        public DateTime? LastConnectedAt { get; set; }
        public DateTime? LastMaintenanceAt { get; set; }
    }
}
