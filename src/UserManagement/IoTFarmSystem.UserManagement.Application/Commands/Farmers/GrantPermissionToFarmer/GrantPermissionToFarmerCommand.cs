using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Commands.Farmers.GrantPermissionToFarmer
{
    public record GrantPermissionToFarmerCommand(Guid FarmerId, string PermissionName) : IRequest<Unit>;
}
