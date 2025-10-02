using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Commands.Roles.CreateRole
{
    public record CreateRoleCommand(string Name) : IRequest<Guid>;
}
