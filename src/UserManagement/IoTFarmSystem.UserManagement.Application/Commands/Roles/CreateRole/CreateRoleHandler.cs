using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Commands.Roles.CreateRole
{
    public class CreateRoleHandler : IRequestHandler<CreateRoleCommand, Guid>
    {
        private readonly IRoleRepository _roleRepository;

        public CreateRoleHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<Guid> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = new Role(Guid.NewGuid(), request.Name);
            await _roleRepository.AddAsync(role, cancellationToken);
            return role.Id;
        }
    }
}
