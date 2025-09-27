namespace IoTFarmSystem.Api.Dtos
{
    public record CreateFarmerByTenantDto(
        string Name,
        string Email,
        string Password,
        IEnumerable<string>? Roles,
        IEnumerable<string>? Permissions
    );
}
