using Ecommerce.Data.Static;
using Ecommerce.Services.Core.IService;
using Ecommerce.ViewModels.Addresses;
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
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;
        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var addresses = await _addressService.GetAddressesByUserId(userId);

                return Ok(new EntityResponseMessage
                {
                    Data = addresses,
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new EntityResponseMessage
                {
                    Message = "Something wrong: " + ex.Message,
                    IsSuccess = false
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Detail(Guid id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var address = await _addressService.GetAddressDetail(userId, id);
                if (address is null)
                    return NotFound(new EntityResponseMessage
                    {
                        Message = $"Address with ID {id} not found.",
                        IsSuccess = false
                    });

                return Ok(new EntityResponseMessage
                {
                    Data = address,
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new EntityResponseMessage
                {
                    Message = "Something wrong: " + ex.Message,
                    IsSuccess = false
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddressAddModel address)
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
                await _addressService.AddNewAddressAsync(userId, address);
                return Ok(new EntityResponseMessage
                {
                    Message = "Address created successfully!",
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var address = await _addressService.GetAddressDetail(userId, id);

            if (address is null)
                return NotFound(new EntityResponseMessage
                {
                    Message = $"Address with ID {id} not found.",
                    IsSuccess = false
                });

            try
            {
                await _addressService.DeleteAsync(id);
                return Ok(new EntityResponseMessage
                {
                    Message = "Address deleted successfully!",
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

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(Guid id, AddressUpdateModel address)
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
                await _addressService.UpdateAddressAsync(address, userId, id);
                return Ok(new EntityResponseMessage
                {
                    Message = "Address updated successfully!",
                    IsSuccess = true
                });

            }
            catch (KeyNotFoundException e)
            {
                return NotFound(new EntityResponseMessage
                {
                    Message = e.Message,
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