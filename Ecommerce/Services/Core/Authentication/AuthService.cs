using Ecommerce.Data.Entities;
using Ecommerce.Services.Core.Catalog;
using Ecommerce.Services.Core.IService;
using Ecommerce.ViewModels.Authentication.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Data;
using System.Security.Claims;
using System.Text;

namespace Ecommerce.Services.Core.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IMailService _mailService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IDriveService _driveService;

        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IMailService mailService, SignInManager<ApplicationUser> signInManager, IDriveService driveService, ITokenService tokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _mailService = mailService;
            _signInManager = signInManager;
            _driveService = driveService;
            _tokenService = tokenService;
        }

        public async Task<UserManagerResponse> ResgisterUserAsync(RegisterViewModel model)
        {
            if (model == null)
            {
                throw new NullReferenceException("Register Model is null");
            }

            if (model.Password != model.ConfirmPassword)
            {
                return new UserManagerResponse
                {
                    Message = "Confirm password doesn't match password",
                    IsSuccess = false,
                };
            }

            var identityUser = new ApplicationUser
            {
                FullName = model.FullName,
                Email = model.Email,
                UserName = model.Email
            };

            var result = await _userManager.CreateAsync(identityUser, model.Password);

            if (result.Succeeded)
            {
                string folderId = _driveService.AddFolder(identityUser.Id, "");

                identityUser.FolderId = folderId;
                await _userManager.UpdateAsync(identityUser);
                // assign role
                if (await _roleManager.RoleExistsAsync("User"))
                    await _userManager.AddToRoleAsync(identityUser, "User");
                // TODO: Send a confirmation email
                var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);

                var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);

                var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

                string url = $"{_configuration["AppUrl"]}/api/auth/confirmemail?userid={identityUser.Id}&token={validEmailToken}";

                await _mailService.SendEmailAsync(identityUser.Email, "Confirm your email", "<h1>Welcome to Auth</h1>" + $"<p>Please confirm your email by <a href='{url}'>Clicking here</a></p>");
                return new UserManagerResponse
                {
                    Message = "User created successfully!",
                    IsSuccess = true,
                };
            }

            return new UserManagerResponse
            {
                Message = "User did not create",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<UserManagerResponse> LoginAsync(LoginViewModel loginViewModel)
        {
            var user = await _userManager.FindByEmailAsync(loginViewModel.Email);
            if (user is null)
                return new UserManagerResponse
                {
                    Message = "There are no user with that Email address",
                    IsSuccess = false,
                };

            var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, true);

            if (result.IsLockedOut)
            {
                var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
                var remainingTime = lockoutEnd.Value.Subtract(DateTimeOffset.Now).Minutes;
                return new UserManagerResponse
                {
                    Message = $"Account is temporarily locked in {remainingTime} minutes",
                    IsSuccess = false
                };
            }
            if (result.IsNotAllowed)
                return new UserManagerResponse
                {
                    Message = "Email not verified. Please check your email and verify your email address before logging in.",
                    IsSuccess = false
                };
            if (!result.Succeeded)
                return new UserManagerResponse
                {
                    Message = "Invalid Password",
                    IsSuccess = false
                };

            var role = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, loginViewModel.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Role, string.Join(";", role))
                };

            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();
            await _tokenService.CreateAsync(refreshToken, user.Id, DateTime.Now.AddDays(7));
            //user.RefreshToken = refreshToken;
            //user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            //await _userManager.UpdateAsync(user);

            return new UserManagerResponse
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                Message = "Success",
                IsSuccess = true
            };
        }

        public async Task<UserManagerResponse> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "User not found"
                };

            var decodedToken = WebEncoders.Base64UrlDecode(token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _userManager.ConfirmEmailAsync(user, normalToken);

            if (result.Succeeded)
                return new UserManagerResponse
                {
                    Message = "Email confirmed successfully!",
                    IsSuccess = true
                };

            return new UserManagerResponse
            {
                IsSuccess = false,
                Message = "Email did not confirm",
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<UserManagerResponse> ForgetPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "No user associated with email"
                };

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodeToken = Encoding.UTF8.GetBytes(token);
            var validToken = WebEncoders.Base64UrlEncode(encodeToken);

            string url = $"{_configuration["AppUrl"]}/ResetPassword?email={email}&token={validToken}";
            await _mailService.SendEmailAsync(email, "Reset Password", "<h1>Follow the instruction to reset your password</h1>" + $"<p>To reset your password <a href={url}>Click here</a></p> ");

            return new UserManagerResponse
            {
                IsSuccess = true,
                Message = "Reset password url has been sent to the email successfully"
            };
        }

        public async Task<UserManagerResponse> ResetPasswordAsync(ResetPasswordViewModel resetPasswordViewModel)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordViewModel.Email);

            if (user is null)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "No user associated with email"
                };

            if (resetPasswordViewModel.NewPassword != resetPasswordViewModel.ConfirmPassword)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "Password doesn't math with its confirmation"
                };

            var decodedToken = WebEncoders.Base64UrlDecode(resetPasswordViewModel.Token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);
            var result = await _userManager.ResetPasswordAsync(user, normalToken, resetPasswordViewModel.NewPassword);

            if (result.Succeeded)
                return new UserManagerResponse
                {
                    Message = "Password has been successfully!",
                    IsSuccess = true
                };

            return new UserManagerResponse
            {
                Message = "Something went wrong",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<IdentityResult> ResgisterAdminAsync(RegisterAdminViewModel model)
        {
            if (model is null)
                return IdentityResult.Failed(new IdentityError { Description = "The data you entered is invalid" });

            if (!model.Password.Equals(model.ConfirmPassword))
                return IdentityResult.Failed(new IdentityError { Description = "Confirm password doesn't match password" });

            var identityUser = new ApplicationUser
            {
                FullName = model.FullName,
                Email = model.Email,
                UserName = model.Email,
            };

            var result = await _userManager.CreateAsync(identityUser, model.Password);

            if (result.Succeeded)
            {
                identityUser.EmailConfirmed = true;
                // assign role
                if (await _roleManager.RoleExistsAsync("Admin"))
                    await _userManager.AddToRoleAsync(identityUser, "Admin");
                // TODO: Send a confirmation email
                var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);

                var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);

                var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

                string url = $"{_configuration["AppUrl"]}/api/auth/confirmemail?userid={identityUser.Id}&token={validEmailToken}";

                await _mailService.SendEmailAsync(identityUser.Email, "Confirm your email", "<h1>Welcome to Auth</h1>" + $"<p>Please confirm your email by <a href='{url}'>Clicking here</a></p>");
                return IdentityResult.Success;
            }

            return IdentityResult.Failed(new IdentityError { Description = "User didn't create." });
        }
    }
}