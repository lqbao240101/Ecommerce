using Ecommerce.Data.Static;
using Ecommerce.Services.Core.IService;
using Ecommerce.ViewModels.CartItem;
using Ecommerce.ViewModels.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.Application.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Authorize(Roles = UserRoles.User)]
    [ApiController]
    [Route("[controller]")]
    public class CartItemController : ControllerBase
    {
        private readonly ICartItemService _cartItemService;

        public CartItemController(ICartItemService cartItemService)
        {
            _cartItemService = cartItemService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CartItemAddModel data)
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
                await _cartItemService.AddNewCartItemAsync(userId, data);
                return Ok(new EntityResponseMessage
                {
                    Message = "Cart Item created successfully!",
                    IsSuccess = true
                });
            }
            catch (Exception de)
            {
                return StatusCode(500, new EntityResponseMessage
                {
                    Message = de.Message,
                    IsSuccess = false
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartItem = await _cartItemService.GetCartItemsByUserId(userId);
            return Ok(new EntityResponseMessage
            {
                Data = cartItem,
                IsSuccess = true
            });
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> Detail(Guid productId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartItem = await _cartItemService.GetCartItemDetail(userId, productId);
            if (cartItem is null)
                return NotFound(new EntityResponseMessage
                {
                    Message = $"Cart Item with ID {productId} not found.",
                    IsSuccess = false
                });

            return Ok(new EntityResponseMessage
            {
                Data = cartItem,
                IsSuccess = true
            });
        }

        [HttpPatch("{productId}")]
        public async Task<IActionResult> Update(Guid productId, CartItemUpdateModel cartItem)
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
                await _cartItemService.UpdateCartItemAsync(cartItem, userId, productId);
                return Ok(new EntityResponseMessage
                {
                    Message = "Cart Item updated successfully!",
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

        [HttpDelete("{productId}")]
        public async Task<IActionResult> Delete(Guid productId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                await _cartItemService.DeleteCartItemAsync(userId, productId);
                return Ok(new EntityResponseMessage
                {
                    Message = "Cart Item deleted successfully!",
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