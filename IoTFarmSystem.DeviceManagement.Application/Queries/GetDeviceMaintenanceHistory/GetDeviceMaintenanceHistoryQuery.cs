using IoTFarmSystem.DeviceManagement.Application.Dtos;
using IoTFarmSystem.SharedKernel.Abstractions;
using MediatR;


namespace IoTFarmSystem.DeviceManagement.Application.Queries.GetDeviceMaintenanceHistory
{
    public record GetDeviceMaintenanceHistoryQuery(Guid DeviceId) : IRequest<Result<List<MaintenanceRecordDto>>>;
}
