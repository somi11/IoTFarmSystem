using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Commands.Roles.AddPermissionToRole
{
    public class AddPermissionToRoleHandler : IRequestHandler<AddPermissionToRoleCommand , Unit>
    {
        private readonly IRoleRepository _roleRepository;

        public AddPermissionToRoleHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<Unit> Handle(AddPermissionToRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
            if (role is null) throw new KeyNotFoundException("Role not found");

            // Update aggregate
            role.AddPermission(request.Permission);

            // Persist through repo
            await _roleRepository.AddPermissionAsync(role, request.Permission, cancellationToken);

            return Unit.Value;
        }
    }
}
