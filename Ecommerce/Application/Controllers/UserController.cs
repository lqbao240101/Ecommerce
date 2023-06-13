using Ecommerce.Data.Static;
using Ecommerce.Services.Core.IService;
using Ecommerce.ViewModels.Authentication.Users;
using Ecommerce.ViewModels.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.Application.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = UserRoles.SuperAdmin + "," + UserRoles.Admin)]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUserByRole(UserRoles.User);
                return Ok(new EntityResponseMessage
                {
                    Data = users,
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new EntityResponseMessage
                {
                    Message = ex.Message,
                    IsSuccess = false
                });
            }
        }

        [Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpGet("Admins")]
        public async Task<IActionResult> GetAllAdmins()
        {
            try
            {
                var admins = await _userService.GetAllUserByRole(UserRoles.Admin);
                return Ok(new EntityResponseMessage
                {
                    Data = admins,
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new EntityResponseMessage
                {
                    Message = ex.Message,
                    IsSuccess = false
                });
            }
        }

        [Authorize]
        [HttpPatch("ChangeAvatar")]
        public async Task<IActionResult> ChangeAvatar(IFormFile file)
        {
            if (file == null)
                return BadRequest(new EntityResponseMessage
                {
                    Message = "You didn't choose any image",
                    IsSuccess = false
                });

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                await _userService.ChangeAvatar(userId, file);

                return Ok(new EntityResponseMessage
                {
                    Message = "Change avatar successfully",
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new EntityResponseMessage
                {
                    Message = ex.Message,
                    IsSuccess = false
                });
            }
        }

        [Authorize]
        [HttpPatch("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel data)
        {
            if (!ModelState.IsValid)
                return BadRequest(new EntityResponseMessage
                {
                    Message = "The data you entered is invalid.",
                    IsSuccess = false
                });

            if (data.NewPassword != data.ConfirmPassword)
            {
                return BadRequest(new EntityResponseMessage
                {
                    Message = "New password and confirm password must be the same.",
                    IsSuccess = false
                });
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var result = await _userService.ChangePassword(userId, data);
                if (result.Succeeded)
                {
                    return Ok(new EntityResponseMessage
                    {
                        Message = "Change password successfully",
                        IsSuccess = true
                    });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return BadRequest(ModelState);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new EntityResponseMessage
                {
                    Message = ex.Message,
                    IsSuccess = false
                });
            }
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var result = await _userService.GetUserById(id);

            if (result == null)
                return NotFound(new EntityResponseMessage
                {
                    Message = $"User with ID {id} not found.",
                    IsSuccess = false
                });

            return Ok(new EntityResponseMessage
            {
                Data = result,
                IsSuccess = true
            });
        }

        [Authorize]
        [HttpGet("PersonalInfomation")]
        public async Task<IActionResult> GetPersonalInfo()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _userService.GetUserById(userId);
            if (result == null)
                return NotFound(new EntityResponseMessage
                {
                    Message = $"User with ID {userId} not found.",
                    IsSuccess = false
                });

            return Ok(new EntityResponseMessage
            {
                Data = result,
                IsSuccess = true
            });
        }

        [Authorize]
        [HttpPatch("ChangeFullName")]
        public async Task<IActionResult> ChangeFullName(string fullName)
        {
            if (fullName == null)
                return BadRequest(new EntityResponseMessage
                {
                    Message = "Full name is required",
                    IsSuccess = false
                });

            if (fullName.Length < 2)
                return BadRequest(new EntityResponseMessage
                {
                    Message = "Full name must have at least 2 characters",
                    IsSuccess = false
                });

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _userService.ChangeFullName(userId, fullName);

            try
            {
                if (result.Succeeded)
                    return Ok(new EntityResponseMessage
                    {
                        Message = "Change full name successfully",
                        IsSuccess = true
                    });
                else
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new EntityResponseMessage
                {
                    Message = ex.Message,
                    IsSuccess = false
                });
            }
        }

        [Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpPatch("ChangePasswordBySuperAdmin/{id}")]
        public async Task<IActionResult> ChangePasswordBySuperAdmin(string id, ChangePasswordBySuperAdminModel data)
        {
            if (!ModelState.IsValid)
                return BadRequest(new EntityResponseMessage
                {
                    Message = "The data you entered is invalid.",
                    IsSuccess = false
                });

            if (data.NewPassword != data.ConfirmPassword)
                return BadRequest(new EntityResponseMessage
                {
                    Message = "New password and confirm password must be the same.",
                    IsSuccess = false
                });

            try
            {
                var result = await _userService.ChangePasswordBySuperAdmin(id, data);
                if (result.Succeeded)
                    return Ok(new EntityResponseMessage
                    {
                        Message = "Change password successfully",
                        IsSuccess = true
                    });
                else
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new EntityResponseMessage
                {
                    Message = ex.Message,
                    IsSuccess = false
                });
            }
        }

        [Authorize(Roles = UserRoles.SuperAdmin + "," + UserRoles.Admin)]
        [HttpPatch("UnlockAccount")]
        public async Task<IActionResult> UnlockAccount(string userName)
        {
            if (userName is null)
                return BadRequest("User name is required");

            try
            {
                var result = await _userService.UnlockUser(userName);
                if (result.Succeeded)
                    return Ok(new EntityResponseMessage
                    {
                        Message = $"Unlock account '{userName}' successfully",
                        IsSuccess = true
                    });
                else
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new EntityResponseMessage
                {
                    Message = ex.Message,
                    IsSuccess = false
                });
            }
        }

        [HttpPost("AddPhoneNumber")]
        public async Task<IActionResult> AddPhone(string phoneNumber)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var result = await _userService.AddPhoneNumber(phoneNumber, userId);
                if (result.Succeeded)
                    return Ok(new EntityResponseMessage
                    {
                        Message = "Verification message has been sent to your phone number",
                        IsSuccess = true
                    });

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return BadRequest(ModelState);
            }
            catch (Exception e)
            {
                return StatusCode(500, new EntityResponseMessage
                {
                    Message = e.Message,
                    IsSuccess = false
                });
            }
        }

        [HttpPost("ConfirmPhone")]
        public async Task<IActionResult> ConfirmPhoneNumber(string VerificationCode)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var result = await _userService.ConfirmPhoneNumber(VerificationCode, userId);
                if (result.Succeeded)
                    return Ok(new EntityResponseMessage
                    {
                        Message = "Thank you for confirming your phone number.",
                        IsSuccess = true
                    });

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return BadRequest(ModelState);
            }
            catch (Exception e)
            {
                return StatusCode(500, new EntityResponseMessage
                {
                    Message = e.Message,
                    IsSuccess = false
                });
            }
        }
    }
}