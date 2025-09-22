using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Queries.Roles.GetRoleById
{
    public class GetRoleByIdHandler : IRequestHandler<GetRoleByIdQuery, Role?>
    {
        private readonly IRoleRepository _roleRepository;

        public GetRoleByIdHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<Role?> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            return await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        }
    }
}
