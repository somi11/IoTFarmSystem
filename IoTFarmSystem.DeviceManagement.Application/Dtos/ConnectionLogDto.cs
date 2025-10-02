using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.DeviceManagement.Application.Dtos
{
    public class ConnectionLogDto
    {
        public Guid Id { get; set; }
        public Guid DeviceId { get; set; }
        public string EventType { get; set; } = default!; // "Connected", "Disconnected", "Error", etc.
        public string Message { get; set; } = default!;
        public DateTime Timestamp { get; set; }

        // Optional metadata for API queries
        public string? DeviceName { get; set; } // From Device aggregate
    }
}
