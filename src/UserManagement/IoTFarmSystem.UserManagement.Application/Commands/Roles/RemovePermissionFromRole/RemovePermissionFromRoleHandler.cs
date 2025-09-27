using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Commands.Roles.RemovePermissionFromRole
{
    public class RemovePermissionFromRoleHandler : IRequestHandler<RemovePermissionFromRoleCommand , Unit>
    {
        private readonly IRoleRepository _roleRepository;

        public RemovePermissionFromRoleHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<Unit> Handle(RemovePermissionFromRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
            if (role is null) throw new KeyNotFoundException("Role not found");

            // Persist removal through repo
            await _roleRepository.RemovePermissionAsync(role, request.Permission, cancellationToken);

            return Unit.Value;
        }
    }
}
