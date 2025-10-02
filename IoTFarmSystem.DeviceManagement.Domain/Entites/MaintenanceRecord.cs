using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.DeviceManagement.Domain.Entites
{       /// <summary>
        /// MaintenanceRecord - Entity
        /// Tracks maintenance activities performed on a device
        /// </summary>
    public class MaintenanceRecord
    {
        public Guid Id { get; private set; }
        public Guid DeviceId { get; private set; }
        public string MaintenanceType { get; private set; } // "RoutineInspection", "Repair", "FirmwareUpdate", "Cleaning", "Replacement"
        public string Description { get; private set; }
        public string PerformedBy { get; private set; } // UserId or technician name
        public DateTime PerformedAt { get; private set; }
        public string? Notes { get; private set; }

        private MaintenanceRecord() { } // EF Core

        public MaintenanceRecord(
            Guid id,
            Guid deviceId,
            string maintenanceType,
            string description,
            string performedBy,
            DateTime performedAt,
            string? notes = null)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Maintenance record Id cannot be empty", nameof(id));
            if (string.IsNullOrWhiteSpace(maintenanceType))
                throw new ArgumentException("Maintenance type is required", nameof(maintenanceType));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description is required", nameof(description));

            Id = id;
            DeviceId = deviceId;
            MaintenanceType = maintenanceType;
            Description = description;
            PerformedBy = performedBy;
            PerformedAt = performedAt;
            Notes = notes;
        }

        public void AddNotes(string notes)
        {
            Notes = notes;
        }

        public void UpdateDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty", nameof(description));

            Description = description;
        }
    }
}
