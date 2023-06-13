using Ecommerce.Data.Entities;
using Ecommerce.Data.Static;
using Ecommerce.Services.Core.IService;
using Ecommerce.ViewModels.Authentication.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.Application.Controllers
{
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Authorize]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        public TokenController(ITokenService tokenService, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }
        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(TokenApiModel tokenApiModel)
        {
            if (tokenApiModel is null)
                return BadRequest("Invalid client request");
            string accessToken = tokenApiModel.AccessToken;
            string refreshToken = tokenApiModel.RefreshToken;
            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            //var username = principal.Identity.Name; //this is mapped to the Name claim by default
            // var user = await _userManager.FindByNameAsync(username);
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var oldToken = await _tokenService.GetOne(refreshToken, userId);
            //if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            if (oldToken is null || oldToken.RefreshTokenExpiryTime <= DateTime.Now)
                return BadRequest("Invalid client request");
            var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            await _tokenService.UpdateAsync(oldToken, newRefreshToken);
            //oldToken.RefreshToken = newRefreshToken;
            //await _userManager.UpdateAsync(user);
            return Ok(new UserManagerResponse()
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken,
                IsSuccess = true
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(string token)
        {
            try
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                bool x = await _tokenService.DeleteOneAsync(token, userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new UserManagerResponse()
                {
                    Message = ex.Message,
                    IsSuccess = false
                });
            }
        }

        [Authorize(Roles = UserRoles.SuperAdmin + "," + UserRoles.Admin)]
        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke(string token, string userId)
        {
            try
            {
                bool x = await _tokenService.DeleteOneAsync(token, userId);
                if (x)
                    return Ok();
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new UserManagerResponse()
                {
                    Message = ex.Message,
                    IsSuccess = false
                });
            }
        }

        [HttpGet("logoutAll")]
        public async Task<IActionResult> LogoutAll()
        {
            try
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _tokenService.DeleteManyAsync(userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new UserManagerResponse()
                {
                    Message = ex.Message,
                    IsSuccess = false
                });
            }
        }
    }
}