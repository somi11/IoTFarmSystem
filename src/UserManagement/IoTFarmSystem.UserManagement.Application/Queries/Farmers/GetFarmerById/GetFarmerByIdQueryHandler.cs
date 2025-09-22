using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Queries.Farmers.GetFarmerById
{
    public class GetFarmerByIdQueryHandler : IRequestHandler<GetFarmerByIdQuery, Farmer?>
    {
        private readonly IFarmerRepository _farmerRepository;

        public GetFarmerByIdQueryHandler(IFarmerRepository farmerRepository)
        {
            _farmerRepository = farmerRepository;
        }

        public async Task<Farmer?> Handle(GetFarmerByIdQuery request, CancellationToken cancellationToken) =>
            await _farmerRepository.GetByIdAsync(request.FarmerId, cancellationToken);
    }
}
