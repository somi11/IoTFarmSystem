using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.DeviceManagement.Application.Dtos
{
    public class MaintenanceRecordDto
    {
        public Guid Id { get; set; }
        public Guid DeviceId { get; set; }
        public string MaintenanceType { get; set; } = default!; // e.g., "Repair", "FirmwareUpdate"
        public string Description { get; set; } = default!;
        public string PerformedBy { get; set; } = default!; // Could be UserId or Technician name
        public DateTime PerformedAt { get; set; }
        public string? Notes { get; set; }

        // Optional extra metadata (good for APIs)
        public string? DeviceName { get; set; } // comes from Device aggregate if included
    }
}
