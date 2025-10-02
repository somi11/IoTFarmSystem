namespace IoTFarmSystem.Api.Dtos
{

    public record ResetPasswordRequest(string Email, string Token, string NewPassword);
}
