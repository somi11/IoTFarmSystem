using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Commands.Roles.UpdateRole
{
    public class UpdateRoleHandler : IRequestHandler<UpdateRoleCommand , Unit>
    {
        private readonly IRoleRepository _roleRepository;

        public UpdateRoleHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<Unit> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
            if (role is null) throw new KeyNotFoundException("Role not found");

            // Rehydrate via reflection or update method in Role aggregate
            typeof(Role)
                .GetProperty("Name")!
                .SetValue(role, request.NewName);

            await _roleRepository.UpdateAsync(role, cancellationToken);
            return Unit.Value;
        }
    }
}
