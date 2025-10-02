using IoTFarmSystem.Api.Dtos;
using IoTFarmSystem.UserManagement.Application.Contracts.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace IoTFarmSystem.Host.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Login endpoint - returns JWT if successful
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            var token = await _authService.AuthenticateAsync(request.Email, request.Password, cancellationToken);
            if (token == null)
                return Unauthorized(new { message = "Invalid email or password" });

            return Ok(new { token });
        }

        /// <summary>
        /// Initiates forgot password process
        /// </summary>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
        {
            var token = await _authService.ForgotPasswordAsync(request.Email, cancellationToken);

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Could not process forgot password request" });

            // In real apps, you would send this via email. For now, return in API response
            return Ok(new
            {
                message = "Password reset link generated",
                token = token
            });
        }
        /// <summary>
        /// Resets user password with token
        /// </summary>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
        {
            var success = await _authService.ResetPasswordAsync(request.Email, request.Token, request.NewPassword, cancellationToken);
            if (!success)
                return BadRequest(new { message = "Invalid token or unable to reset password" });

            return Ok(new { message = "Password has been reset successfully" });
        }
    }

}
