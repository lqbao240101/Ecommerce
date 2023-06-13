using Microsoft.AspNetCore.Identity;
using Ecommerce.ViewModels.Authentication.Users;
using Ecommerce.ViewModels.ViewModels.MapperModel;

namespace Ecommerce.Services.Core.IService
{
    public interface IUserService
    {
        Task ChangeAvatar(string userId, IFormFile file);
        Task<IEnumerable<GetAllUsersByAdminView>> GetAllUserByRole(string role);
        Task<IdentityResult> ChangePassword(string userId, ChangePasswordModel data);
        Task<GetUserByIdView> GetUserById(string userId);
        Task<IdentityResult> ChangeFullName(string userId, string fullName);
        Task<IdentityResult> ChangePasswordBySuperAdmin(string userId, ChangePasswordBySuperAdminModel data);
        Task<IdentityResult> UnlockUser(string userName);
        Task<IdentityResult> AddPhoneNumber(string phoneNumber, string userId);
        Task<IdentityResult> ConfirmPhoneNumber(string VerificationCode, string userId);
    }
}