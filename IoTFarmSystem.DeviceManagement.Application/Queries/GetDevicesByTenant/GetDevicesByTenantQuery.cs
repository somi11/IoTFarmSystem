using IoTFarmSystem.DeviceManagement.Application.Dtos;
using IoTFarmSystem.SharedKernel.Abstractions;
using MediatR;


namespace IoTFarmSystem.DeviceManagement.Application.Queries.GetDevicesByTenant
{
    public record GetDevicesByTenantQuery(Guid TenantId) : IRequest<Result<List<DeviceDto>>>;
}
