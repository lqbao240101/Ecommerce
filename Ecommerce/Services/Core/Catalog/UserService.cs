using AutoMapper;
using Ecommerce.Data.Entities;
using Ecommerce.Services.Core.IService;
using Ecommerce.ViewModels.Authentication.Users;
using Ecommerce.ViewModels.ViewModels.MapperModel;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Services.Core.Catalog
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IDriveService _driveService;
        private readonly ISMSService _smsService;

        public UserService(UserManager<ApplicationUser> userManager, IDriveService driveService, IMapper mapper, ISMSService smsService)
        {
            _userManager = userManager;
            _driveService = driveService;
            _mapper = mapper;
            _smsService = smsService;
        }
        public async Task ChangeAvatar(string userId, IFormFile file)
        {
            var user = await _userManager.FindByIdAsync(userId);

            string[] s = file.FileName.Split('.');
            string ex = "." + s[s.Length - 1];
            var filePath = "DriveImage/download" + ex;
            if (file.Length > 0)
            {
                using var stream = File.Create(filePath);
                await file.CopyToAsync(stream);
            }

            string imageUrl = _driveService.AddFile(filePath, ex, user.FolderId);

            if (!string.IsNullOrEmpty(user.Avatar))
            {
                string url = Uri.UnescapeDataString(user.Avatar).ToString();
                string id = url.Split(new[] { "id=" }, StringSplitOptions.None)[1];
                await _driveService.Remove(id);
            }
            user.Avatar = imageUrl;
            await _userManager.UpdateAsync(user);
        }
        public async Task<IEnumerable<GetAllUsersByAdminView>> GetAllUserByRole(string role)
        {
            var users = _mapper.Map<IEnumerable<GetAllUsersByAdminView>>(await _userManager.GetUsersInRoleAsync(role)).ToArray();
            return users;
        }
        public async Task<IdentityResult> ChangePassword(string userId, ChangePasswordModel data)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            var result = await _userManager.ChangePasswordAsync(user, data.CurrentPassword, data.NewPassword);

            return result;
        }

        public async Task<IdentityResult> ChangePasswordBySuperAdmin(string userId, ChangePasswordBySuperAdminModel data)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, token, data.NewPassword);

            return result;
        }

        public async Task<GetUserByIdView> GetUserById(string userId)
        {
            var user = _mapper.Map<GetUserByIdView>(await _userManager.FindByIdAsync(userId));
            return user;
        }

        public async Task<IdentityResult> ChangeFullName(string userId, string fullName)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            user.FullName = fullName;
            var result = await _userManager.UpdateAsync(user);
            return result;
        }

        public async Task<IdentityResult> UnlockUser(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user is null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            if (user is not null && await _userManager.IsLockedOutAsync(user))
            {
                await _userManager.ResetAccessFailedCountAsync(user);
                await _userManager.SetLockoutEndDateAsync(user, null);
                return IdentityResult.Success;
            }

            return IdentityResult.Failed(new IdentityError { Description = "User is not locked." });
        }

        public async Task<IdentityResult> AddPhoneNumber(string phoneNumber, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return IdentityResult.Failed(new IdentityError { Description = $"User not found. {userId}" });

            user.PhoneNumber = phoneNumber;
            var updateResult = await _userManager.UpdateAsync(user);

            if (updateResult.Succeeded)
            {
                var result = await _smsService.VerifyPhone(phoneNumber);
                if (result == "pending")
                    return IdentityResult.Success;

                return IdentityResult.Failed(new IdentityError { Description = $"There was an error sending the verification code: {result}" });
            }

            return IdentityResult.Failed(new IdentityError { Description = "Something wrong" });
        }

        public async Task<IdentityResult> ConfirmPhoneNumber(string VerificationCode, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return IdentityResult.Failed(new IdentityError { Description = $"User not found. {userId}" });

            var check = await _smsService.ConfirmPhone(VerificationCode, user.PhoneNumber);

            if (check.Succeeded)
            {
                user.PhoneNumberConfirmed = true;
                var updateResult = await _userManager.UpdateAsync(user);

                if (updateResult.Succeeded)
                    return IdentityResult.Success;
                else
                    return IdentityResult.Failed(new IdentityError { Description = "There was an error confirming the verification code, please try again" });
            }

            return IdentityResult.Failed(new IdentityError { Description = $"{check}" });
        }
    }
}