using Ecommerce.Services.Core.IService;
using Ecommerce.ViewModels.Common;
using Ecommerce.ViewModels.EntityParameters;
using Ecommerce.ViewModels.Reviews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Ecommerce.Application.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [AllowAnonymous]
        [HttpGet("{productId}")]
        public IActionResult GetAll(Guid productId, [FromQuery] ReviewParameters parameters)
        {
            try
            {
                var reviews = _reviewService.GetReviewsWithPaging(productId, parameters);

                if (reviews is null)
                    return NotFound(new EntityResponseMessage
                    {
                        Message = $"Product with ID {productId} not found.",
                        IsSuccess = false
                    }); ;

                var metadata = new
                {
                    reviews.TotalCount,
                    reviews.PageSize,
                    reviews.CurrentPage,
                    reviews.TotalPages,
                    reviews.HasNext,
                    reviews.HasPrevious
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                return Ok(new EntityResponseMessage
                {
                    Data = reviews,
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

        [HttpPost]
        public async Task<IActionResult> CreateReview(ReviewAddModel data)
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
                await _reviewService.CreateAsync(userId, data);
                return Ok(new EntityResponseMessage
                {
                    Message = "Review created successfully!",
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
        public async Task<IActionResult> ChangeReview(Guid id, ReviewUpdateModel data)
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
                await _reviewService.UpdateAsync(userId, data, id);
                return Ok(new EntityResponseMessage
                {
                    Message = "Review updated successfully!",
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
        public async Task<IActionResult> DeleteReview(Guid id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                await _reviewService.DeleteAsync(userId, id);
                return Ok(new EntityResponseMessage
                {
                    Message = "Review deleted successfully!",
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