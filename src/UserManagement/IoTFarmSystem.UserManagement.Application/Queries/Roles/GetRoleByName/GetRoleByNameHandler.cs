using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Queries.Roles.GetRoleByName
{
    public class GetRoleByNameHandler : IRequestHandler<GetRoleByNameQuery, Role?>
    {
        private readonly IRoleRepository _roleRepository;

        public GetRoleByNameHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<Role?> Handle(GetRoleByNameQuery request, CancellationToken cancellationToken)
        {
            return await _roleRepository.GetByNameAsync(request.RoleName, cancellationToken);
        }
    }
}
