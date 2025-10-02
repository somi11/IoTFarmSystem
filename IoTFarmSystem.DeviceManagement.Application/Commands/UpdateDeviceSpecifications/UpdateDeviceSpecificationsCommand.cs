using IoTFarmSystem.SharedKernel.Abstractions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.DeviceManagement.Application.Commands.UpdateDeviceSpecifications
{
    public record UpdateDeviceSpecificationsCommand(
     Guid DeviceId,
     int MaxIntensity,
     int PowerRatingWatts,
     string LightSpectrum,
     string Manufacturer,
     string ModelNumber
 ) : IRequest<Result>;
}
