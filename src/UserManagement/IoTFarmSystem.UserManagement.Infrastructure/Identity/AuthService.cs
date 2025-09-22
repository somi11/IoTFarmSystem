using IoTFarmSystem.UserManagement.Application.Contracts.Identity;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace IoTFarmSystem.UserManagement.Infrastructure.Identity
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;  // <-- custom user manager
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IJwtService _jwtService;

        public AuthService(
            IUserService userService,
            SignInManager<IdentityUser> signInManager,
            IJwtService jwtService)
        {
            _userService = userService;
            _signInManager = signInManager;
            _jwtService = jwtService;
        }

        public async Task<string?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            // 1. Lookup user
            var userExists = await _userService.UserExistsAsync(email, cancellationToken);
            if (!userExists) return null;

            var identityUser = await _signInManager.UserManager.FindByEmailAsync(email);
            if (identityUser == null) return null;

            // 2. Verify password
            var result = await _signInManager.CheckPasswordSignInAsync(identityUser, password, false);
            if (!result.Succeeded) return null;

            // 3. Delegate token generation to JwtService
            return await _jwtService.GenerateTokenAsync(identityUser.Id, cancellationToken);
        }

        public async Task<bool> ForgotPasswordAsync(string email, CancellationToken cancellationToken = default)
        {
            var userExists = await _userService.UserExistsAsync(email, cancellationToken);
            if (!userExists) return false;

            var identityUser = await _signInManager.UserManager.FindByEmailAsync(email);
            if (identityUser == null) return false;

            var token = await _signInManager.UserManager.GeneratePasswordResetTokenAsync(identityUser);

            // TODO: send token via EmailService
            return true;
        }

        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken = default)
        {
            var identityUser = await _signInManager.UserManager.FindByEmailAsync(email);
            if (identityUser == null) return false;

            var result = await _signInManager.UserManager.ResetPasswordAsync(identityUser, token, newPassword);
            return result.Succeeded;
        }
    }
}
