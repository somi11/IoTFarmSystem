using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using IoTFarmSystem.UserManagement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Queries.Farmers.GetFarmerByEmail
{
    public class GetFarmerByEmailQueryHandler : IRequestHandler<GetFarmerByEmailQuery, FarmerDto?>
    {
        private readonly IFarmerRepository _farmerRepository;

        public GetFarmerByEmailQueryHandler(IFarmerRepository farmerRepository)
        {
            _farmerRepository = farmerRepository;
        }

        public async Task<FarmerDto?> Handle(GetFarmerByEmailQuery request, CancellationToken cancellationToken) =>
            await _farmerRepository.GetByEmailAsync(request.Email, cancellationToken);
    }
}
