using IoTFarmSystem.SharedKernel.Abstractions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Commands.Farmers.SignUpFarmer
{
    public record SignUpFarmerCommand(
        string Name,
        string Email,
        string Password
    ) : IRequest<Result<Guid>>;
}
