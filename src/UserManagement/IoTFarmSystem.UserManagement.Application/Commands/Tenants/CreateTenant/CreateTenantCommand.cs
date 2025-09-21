using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Commands.Tenants.CreateTenantCommand
{
    public record CreateTenantCommand(string Name) : IRequest<Guid>;
}
