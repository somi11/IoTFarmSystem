using IoTFarmSystem.SharedKernel.Abstractions;
using MediatR;


namespace IoTFarmSystem.DeviceManagement.Application.Commands.UpdateFirmware
{
    public record UpdateFirmwareCommand(
       Guid DeviceId,
       string FirmwareVersion
   ) : IRequest<Result>;
}
