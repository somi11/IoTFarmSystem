using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using IoTFarmSystem.UserManagement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Queries.Farmers.GetFarmersByRole
{
    public class GetFarmersByRoleQueryHandler : IRequestHandler<GetFarmersByRoleQuery, IReadOnlyList<FarmerDto>>
    {
        private readonly IFarmerRepository _farmerRepository;

        public GetFarmersByRoleQueryHandler(IFarmerRepository farmerRepository)
        {
            _farmerRepository = farmerRepository;
        }

        public async Task<IReadOnlyList<FarmerDto>> Handle(GetFarmersByRoleQuery request, CancellationToken cancellationToken) =>
            await _farmerRepository.GetByRoleNameAsync(request.TenantId, request.RoleName, cancellationToken);
    }
}
