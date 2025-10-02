using IoTFarmSystem.SharedKernel.Abstractions;
using IoTFarmSystem.UserManagement.Application.Contracts.Persistance;
using IoTFarmSystem.UserManagement.Application.Contracts.Repositories;
using MediatR;

namespace IoTFarmSystem.UserManagement.Application.Commands.Tenants.CreateTenantCommand
{
    public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, Result<Guid>>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateTenantCommandHandler(
            ITenantRepository tenantRepository,
            IUnitOfWork unitOfWork)
        {
            _tenantRepository = tenantRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                // 1. Create tenant aggregate
                var tenant = new Tenant(Guid.NewGuid(), request.Name);

                // 2. Track it
                await _tenantRepository.AddAsync(tenant, cancellationToken);

                // 3. Commit once (saves + commits transaction)
                await transaction.CommitAsync(cancellationToken);

                return Result<Guid>.Ok(tenant.Id);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result<Guid>.Fail($"Unexpected error: {ex.Message}");
            }
        }
    }
}
