using IoTFarmSystem.SharedKernel.Abstractions;
using IoTFarmSystem.SharedKernel.Security;
using IoTFarmSystem.UserManagement.Application.Commands.Farmers.CreateFarmer;
using IoTFarmSystem.UserManagement.Application.Commands.Farmers.SignUpFarmer;
using IoTFarmSystem.UserManagement.Application.Commands.Tenants.CreateTenantCommand;
using MediatR;

public class SignUpFarmerCommandHandler
    : IRequestHandler<SignUpFarmerCommand, Result<Guid>>
{
    private readonly IMediator _mediator;

    public SignUpFarmerCommandHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<Result<Guid>> Handle(SignUpFarmerCommand request, CancellationToken cancellationToken)
    {
        // 1. Create Tenant (aggregate root)
        var tenantName = $"{request.Name}'s Farm";
        var tenantResult = await _mediator.Send(new CreateTenantCommand(tenantName), cancellationToken);
        if (!tenantResult.Success)
            return Result<Guid>.Fail(tenantResult.Error);

        var tenantId = tenantResult.Value;

        // 2. Create Farmer inside that tenant
        var farmerResult = await _mediator.Send(
            new CreateFarmerCommand(
                TenantId: tenantId,
                Name: request.Name,
                Email: request.Email,
                Password: request.Password,
                Roles: new[] { SystemRoles.TENANT_OWNER } // always owner in sign-up flow
            ),
            cancellationToken);

        return farmerResult;
    }
}
