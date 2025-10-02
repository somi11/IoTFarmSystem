using IoTFarmSystem.SharedKernel.Abstractions;
using MediatR;


namespace IoTFarmSystem.DeviceManagement.Application.Commands.ActivateDevice
{
    public record ActivateDeviceCommand(Guid DeviceId, Guid UserId) : IRequest<Result>;
}
