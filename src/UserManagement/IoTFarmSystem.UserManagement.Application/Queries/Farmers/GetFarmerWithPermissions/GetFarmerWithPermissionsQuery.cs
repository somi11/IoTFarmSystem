﻿using IoTFarmSystem.UserManagement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Queries.Farmers.GetFarmerWithPermissions
{
    public record GetFarmerWithPermissionsQuery(Guid FarmerId) : IRequest<FarmerDto?>;
}
