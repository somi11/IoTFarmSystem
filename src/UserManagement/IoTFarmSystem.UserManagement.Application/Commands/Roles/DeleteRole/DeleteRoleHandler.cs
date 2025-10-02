using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Commands.Roles.DeleteRole
{
    public class DeleteRoleHandler : IRequestHandler<DeleteRoleCommand , Unit>
    {
        private readonly IRoleRepository _roleRepository;

        public DeleteRoleHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<Unit> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
            if (role is null) throw new KeyNotFoundException("Role not found");

            await _roleRepository.DeleteAsync(role, cancellationToken);
            return Unit.Value;
        }
    }
}
