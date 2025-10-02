using IoTFarmSystem.SharedKernel.Abstractions;
using IoTFarmSystem.UserManagement.Application.Contracts.Persistance;
using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Commands.Tenants.UpdateTenant
{
    public class UpdateTenantCommandHandler : IRequestHandler<UpdateTenantCommand, Result<Unit>>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTenantCommandHandler(
           ITenantRepository tenantRepository,
           IUnitOfWork unitOfWork)
        {
            _tenantRepository = tenantRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<Unit>> Handle(UpdateTenantCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var tenant = await _tenantRepository.GetByIdAsync(request.TenantId, cancellationToken);
            if (tenant is null)
                throw new KeyNotFoundException($"Tenant {request.TenantId} not found");

            tenant.UpdateName(request.Name);

            await _tenantRepository.UpdateAsync(tenant, cancellationToken);
                
                await transaction.CommitAsync(cancellationToken);
                return Result<Unit>.Ok(Unit.Value);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result<Unit>.Fail($"Unexpected error: {ex.Message}");
            }
        }
    }
}
