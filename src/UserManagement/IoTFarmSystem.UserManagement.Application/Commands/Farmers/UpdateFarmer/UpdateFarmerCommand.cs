using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Commands.Farmers.UpdateFarmer
{
    public record UpdateFarmerCommand(
         Guid FarmerId,
         string? Name = null,
         string? Email = null
     ) : IRequest<Unit>;
}
