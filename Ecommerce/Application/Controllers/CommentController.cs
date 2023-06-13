using Ecommerce.Services.Core.IService;
using Ecommerce.ViewModels.Comments;
using Ecommerce.ViewModels.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.Application.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment(CommentAddModel data)
        {
            if (!ModelState.IsValid)
                return BadRequest(new EntityResponseMessage
                {
                    Message = "The data you entered is invalid.",
                    IsSuccess = false
                });

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                await _commentService.CreateAsync(userId, data);
                return Ok(new EntityResponseMessage
                {
                    Message = "Comment created successfully!",
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

        [HttpPatch("{id}")]
        public async Task<IActionResult> ChangeComment(Guid id, CommentUpdateModel data)
        {
            if (!ModelState.IsValid)
                return BadRequest(new EntityResponseMessage
                {
                    Message = "The data you entered is invalid.",
                    IsSuccess = false
                });

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                await _commentService.UpdateAsync(userId, data, id);
                return Ok(new EntityResponseMessage
                {
                    Message = "Comment updated successfully!",
                    IsSuccess = true
                });
            }
            catch (KeyNotFoundException k)
            {
                return NotFound(new EntityResponseMessage
                {
                    Message = k.Message,
                    IsSuccess = false
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                await _commentService.DeleteAsync(userId, id);
                return Ok(new EntityResponseMessage
                {
                    Message = "Comment deleted successfully!",
                    IsSuccess = true
                });
            }
            catch (KeyNotFoundException k)
            {
                return NotFound(new EntityResponseMessage
                {
                    Message = k.Message,
                    IsSuccess = false
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
    }
}