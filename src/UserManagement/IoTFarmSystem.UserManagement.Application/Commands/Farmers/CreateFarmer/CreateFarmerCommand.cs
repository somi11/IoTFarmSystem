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
    Guid TenantId,
    string Name,
    string Email,
    string Password,
    IEnumerable<string>? Roles = null,
    IEnumerable<string>? Permissions = null,
    bool IsSelfSignUp = false
) : IRequest<Result<Guid>>;

}
