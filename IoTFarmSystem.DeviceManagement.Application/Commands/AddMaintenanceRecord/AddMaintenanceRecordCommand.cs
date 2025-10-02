using IoTFarmSystem.SharedKernel.Abstractions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.DeviceManagement.Application.Commands.AddMaintenanceRecord
{
    public record AddMaintenanceRecordCommand(
        Guid DeviceId,
        string PerformedBy,
        string MaintenanceType,
        string Description,
        string? Notes = null
    ) : IRequest<Result>;
}
