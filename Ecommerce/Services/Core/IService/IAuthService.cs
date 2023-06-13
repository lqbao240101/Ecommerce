using Ecommerce.ViewModels.Authentication.Users;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Services.Core.IService
{
    public interface IAuthService
    {
        Task<UserManagerResponse> ResgisterUserAsync(RegisterViewModel model);
        Task<IdentityResult> ResgisterAdminAsync(RegisterAdminViewModel model);
        Task<UserManagerResponse> LoginAsync(LoginViewModel loginViewModel);
        Task<UserManagerResponse> ConfirmEmailAsync(string userId, string token);
        Task<UserManagerResponse> ForgetPasswordAsync(string email);
        Task<UserManagerResponse> ResetPasswordAsync(ResetPasswordViewModel resetPasswordViewModel);
    }
}