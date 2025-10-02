using IoTFarmSystem.SharedKernel.Abstractions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.DeviceManagement.Application.Commands.DeleteDevice
{
    public record DeleteDeviceCommand(Guid DeviceId) : IRequest<Result>;
}
