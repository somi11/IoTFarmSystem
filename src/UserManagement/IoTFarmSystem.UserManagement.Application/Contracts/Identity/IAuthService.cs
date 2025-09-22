using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Application.Contracts.Identity
{
    public interface IAuthService
    {
        /// <summary>
        /// Authenticates a user using email and password.
        /// Returns a JWT token string if successful, null otherwise.
        /// </summary>
        Task<string?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);

        /// <summary>
        /// Initiates a forgot password process (sends reset link/email).
        /// </summary>
        Task<bool> ForgotPasswordAsync(string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// Resets the password for a user given a token and new password.
        /// </summary>
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken = default);
    }
}
