namespace IoTFarmSystem.Api.Dtos
{
    public record CreateFarmerByAdminDto(
      Guid TenantId,  // required
      string Name,
      string Email,
      string Password,
      IEnumerable<string>? Roles,
      IEnumerable<string>? Permissions
  );
}
