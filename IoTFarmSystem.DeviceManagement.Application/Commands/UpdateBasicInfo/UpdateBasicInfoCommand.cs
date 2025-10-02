using IoTFarmSystem.SharedKernel.Abstractions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.DeviceManagement.Application.Commands.UpdateBasicInfo
{
    public record UpdateDeviceBasicInfoCommand(
        Guid DeviceId,
        string DeviceName,
        string? Location,
        string? Description
    ) : IRequest<Result>;
}
