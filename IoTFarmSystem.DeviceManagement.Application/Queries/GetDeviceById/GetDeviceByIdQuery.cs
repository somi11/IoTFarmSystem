using IoTFarmSystem.DeviceManagement.Application.Dtos;
using IoTFarmSystem.SharedKernel.Abstractions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.DeviceManagement.Application.Queries.GetDeviceById
{
    public record GetDeviceByIdQuery(Guid DeviceId) : IRequest<Result<DeviceDto>>;
}
