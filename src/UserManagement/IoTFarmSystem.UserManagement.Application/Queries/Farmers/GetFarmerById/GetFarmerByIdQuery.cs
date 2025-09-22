using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Queries.Farmers.GetFarmerById
{
    public record GetFarmerByIdQuery(Guid FarmerId) : IRequest<Farmer?>;
}
