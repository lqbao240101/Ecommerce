using Ecommerce.Data.Static;
using Ecommerce.Services.Core.IService;
using Ecommerce.ViewModels.Authentication.Users;
using Ecommerce.ViewModels.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.ResgisterUserAsync(registerViewModel);

                if (result.IsSuccess)
                    return Ok(result);

                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid");
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpPost("RegisterAdmin")]
        public async Task<IActionResult> RegisterAdminAsync(RegisterAdminViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _authService.ResgisterAdminAsync(registerViewModel);

                    if (result.Succeeded)
                        return Ok(new EntityResponseMessage
                        {
                            Message = "User created successfully!",
                            IsSuccess = true
                        });

                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);


                    return BadRequest(ModelState);
                }
                catch
                {
                    return StatusCode(500, new EntityResponseMessage
                    {
                        Message = "Something wrong",
                        IsSuccess = false
                    });
                }
            }

            return BadRequest(new EntityResponseMessage
            {
                Message = "The data you entered is invalid.",
                IsSuccess = false
            });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.LoginAsync(loginViewModel);
                if (result.IsSuccess)
                    return Ok(result);

                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid");
        }


        // api/auth/confirmemail?userid&token
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return NotFound();
            var result = await _authService.ConfirmEmailAsync(userId, token);

            if (result.IsSuccess)
                return Redirect($"{_configuration["AppUrl"]}/confirmEmail.html");

            return BadRequest(result);
        }

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
                return NotFound();

            var result = await _authService.ForgetPasswordAsync(email);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordViewModel resetPasswordViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.ResetPasswordAsync(resetPasswordViewModel);
                if (result.IsSuccess)
                    return Ok(result);

                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid");
        }
    }
}