using IoTFarmSystem.DeviceManagement.Application.Dtos;
using IoTFarmSystem.SharedKernel.Abstractions;
using MediatR;


namespace IoTFarmSystem.DeviceManagement.Application.Queries.GetDevicesRequiringMaintenance
{
    public record GetDevicesRequiringMaintenanceQuery(int DaysSinceLastMaintenance = 90)
        : IRequest<Result<List<DeviceDto>>>;
}
