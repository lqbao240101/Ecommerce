using Ecommerce.Data.Static;
using Ecommerce.Services.Core.IService;
using Ecommerce.ViewModels.Common;
using Ecommerce.ViewModels.ViewModels.MapperModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.Application.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Authorize(Roles = UserRoles.SuperAdmin + "," + UserRoles.Admin)]
        [HttpGet("Admin")]
        public async Task<IActionResult> GetOrdersByAdmin()
        {
            try
            {
                var orders = await _orderService.GetOrdersByAdmin();
                return Ok(new EntityResponseMessage
                {
                    Data = orders,
                    IsSuccess = true,
                });
            }
            catch (Exception e)
            {
                return StatusCode(500, new EntityResponseMessage
                {
                    Message = "Something wrong: " + e.Message,
                    IsSuccess = false
                });
            }
        }

        [Authorize(Roles = UserRoles.User)]
        [HttpGet]
        public async Task<IActionResult> GetOrdersByUser()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var result = await _orderService.GetOrdersByUser(userId);
                return Ok(new EntityResponseMessage
                {
                    Data = result,
                    IsSuccess = true
                });
            }
            catch (Exception e)
            {
                return StatusCode(500, new EntityResponseMessage
                {
                    Message = "Something wrong: " + e.Message,
                    IsSuccess = false
                });
            }
        }

        [Authorize(Roles = UserRoles.User)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderByUser(Guid id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _orderService.GetOrderByUser(userId, id);
            if (result is null)
                return NotFound(new EntityResponseMessage
                {
                    Message = $"Your order with Id {id} is not found",
                    IsSuccess = false
                });

            return Ok(new EntityResponseMessage
            {
                Data = result,
                IsSuccess = true
            });
        }

        [Authorize(Roles = UserRoles.User)]
        [HttpPost]
        public async Task<IActionResult> MakeOrder([FromQuery] List<Guid> productIds, OrderPost orderview)
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
                await _orderService.CheckBeforeCreateOrderAsync(productIds, orderview, userId);

                return Ok(new EntityResponseMessage
                {
                    Message = $"Đơn hàng của bạn đã tạo thành công",
                    IsSuccess = true
                });
            }
            catch (ArgumentException a)
            {
                return BadRequest(new EntityResponseMessage
                {
                    Message = a.Message,
                    IsSuccess = false
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
            catch (Exception e)
            {
                return StatusCode(500, new EntityResponseMessage
                {
                    Message = e.Message,
                    IsSuccess = false
                });
            }
        }

        [Authorize]
        [HttpPatch("CancelOrder/{id}")]
        public async Task<IActionResult> CancelOrder(Guid id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string role = User.FindFirstValue(ClaimTypes.Role);

            try
            {
                await _orderService.CancelOrder(id, role, userId);
                return Ok(new EntityResponseMessage
                {
                    Message = $"Đơn hàng hủy thành công",
                    IsSuccess = true
                });
            }
            catch (ArgumentException a)
            {
                return BadRequest(new EntityResponseMessage
                {
                    Message = a.Message,
                    IsSuccess = false
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
            catch (Exception e)
            {
                return StatusCode(500, new EntityResponseMessage
                {
                    Message = e.Message,
                    IsSuccess = false
                });
            }
        }

        [Authorize(Roles = UserRoles.SuperAdmin + "," + UserRoles.Admin)]
        [HttpPatch("ConfirmOrder/{id}")]
        public async Task<IActionResult> ConfirmOrder(Guid id)
        {
            try
            {
                await _orderService.ConfirmOrder(id);
                return Ok(new EntityResponseMessage
                {
                    Message = $"Order confirmation successful",
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