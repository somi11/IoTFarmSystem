using IoTFarmSystem.SharedKernel.Abstractions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.DeviceManagement.Application.Commands
{
    public record DeactivateDeviceCommand(Guid DeviceId, Guid UserId) : IRequest<Result>;
}
