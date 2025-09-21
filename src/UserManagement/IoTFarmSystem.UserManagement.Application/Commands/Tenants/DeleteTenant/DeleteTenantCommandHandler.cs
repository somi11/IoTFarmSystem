using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Commands.Tenants.DeleteTenant
{
    public class DeleteTenantCommandHandler : IRequestHandler<DeleteTenantCommand , Unit>
    {
        private readonly ITenantRepository _tenantRepository;

        public DeleteTenantCommandHandler(ITenantRepository tenantRepository)
        {
            _tenantRepository = tenantRepository;
        }

        public async Task<Unit> Handle(DeleteTenantCommand request, CancellationToken cancellationToken)
        {
            var tenant = await _tenantRepository.GetByIdAsync(request.TenantId, cancellationToken);
            if (tenant is null)
                throw new KeyNotFoundException($"Tenant {request.TenantId} not found");

            // if domain rules require: check if tenant has farmers, prevent deletion
            if (tenant.Farmers.Any())
                throw new InvalidOperationException("Tenant cannot be deleted while it has farmers.");

            
            await _tenantRepository.DeleteAsync(tenant, cancellationToken);

            return Unit.Value;
        }
    }
}
