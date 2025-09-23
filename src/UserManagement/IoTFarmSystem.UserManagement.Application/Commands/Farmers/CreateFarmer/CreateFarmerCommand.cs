using IoTFarmSystem.SharedKernel.Abstractions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Commands.Farmers.CreateFarmer
{
    public record CreateFarmerCommand(
       string Name,
       string Email,
       string Password,
       Guid? TenantId,
       string? TenantName = null,   // optional
       IEnumerable<string>? Roles = null,
       IEnumerable<string>? Permissions = null
 ) : IRequest<Result<Guid>>;

}
