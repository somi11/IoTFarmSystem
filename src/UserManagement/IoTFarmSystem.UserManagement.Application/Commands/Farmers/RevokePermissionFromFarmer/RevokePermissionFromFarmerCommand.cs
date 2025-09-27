using IoTFarmSystem.SharedKernel.Abstractions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Commands.Farmers.RevokePermissionFromFarmer
{
    public record RevokePermissionFromFarmerCommand(Guid FarmerId, string PermissionName) : IRequest<Result<Unit>>;

}
