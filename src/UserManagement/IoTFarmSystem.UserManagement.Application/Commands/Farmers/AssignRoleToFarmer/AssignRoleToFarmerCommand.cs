using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Commands.Farmers.AssignRoleToFarmer
{
    public record AssignRoleToFarmerCommand(Guid FarmerId, string RoleName) : IRequest<Unit>;
}
