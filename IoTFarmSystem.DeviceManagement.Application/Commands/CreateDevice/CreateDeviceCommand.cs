using IoTFarmSystem.SharedKernel.Abstractions;
using MediatR;
namespace IoTFarmSystem.DeviceManagement.Application.Commands.CreateDevice
{
    public record CreateDeviceCommand(
        Guid TenantId,
        string DeviceName,
        string SerialNumber,
        string AzureIotDeviceId,
        string ConnectionString,
        string DeviceType,
        int MaxIntensity,
        int PowerRatingWatts,
        string LightSpectrum,
        string Manufacturer,
        string ModelNumber,
        string FirmwareVersion,
        string? Location = null,
        string? Description = null
    ) : IRequest<Result<Guid>>;
}
