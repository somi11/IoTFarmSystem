using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.DeviceManagement.Domain.Entites
{
    /// <summary>
    /// ConnectionLog - Entity
    /// Tracks device connection events for monitoring and troubleshooting
    /// </summary>
    public class ConnectionLog
    {
        public Guid Id { get; private set; }
        public Guid DeviceId { get; private set; }
        public string EventType { get; private set; } // "Connected", "Disconnected", "Error", "Reset", "ConfigurationChanged"
        public string Message { get; private set; }
        public DateTime Timestamp { get; private set; }

        private ConnectionLog() { } // EF Core

        public ConnectionLog(
            Guid id,
            Guid deviceId,
            string eventType,
            string message,
            DateTime timestamp)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Connection log Id cannot be empty", nameof(id));
            if (string.IsNullOrWhiteSpace(eventType))
                throw new ArgumentException("Event type is required", nameof(eventType));
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message is required", nameof(message));

            Id = id;
            DeviceId = deviceId;
            EventType = eventType;
            Message = message;
            Timestamp = timestamp;
        }
    }
}

