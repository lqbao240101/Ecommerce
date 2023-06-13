using Ecommerce.Data.Entities;
using Ecommerce.Data.Static;
using Ecommerce.Services.Core.IService;
using Ecommerce.ViewModels.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Application.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Authorize(Roles = UserRoles.SuperAdmin + "," + UserRoles.Admin)]
    [ApiController]
    [Route("[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var allCategories = await _categoryService.GetAllAsync(n => n.Products);
                return Ok(new EntityResponseMessage
                {
                    Data = allCategories,
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

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> Detail(Guid id)
        {
            var category = await _categoryService.GetByIdAsync(id, n => n.Products);
            if (category is null)
                return NotFound(new EntityResponseMessage
                {
                    Message = $"Category with ID {id} not found.",
                    IsSuccess = false
                });

            return Ok(new EntityResponseMessage
            {
                Data = category,
                IsSuccess = true
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("CategoryName, Description")] Category category)
        {
            if (!ModelState.IsValid)
                return BadRequest(new EntityResponseMessage
                {
                    Message = "The data you entered is invalid.",
                    IsSuccess = false
                });

            try
            {
                await _categoryService.AddAsync(category);
                return Ok(new EntityResponseMessage
                {
                    Message = "Category created successfully!",
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
            try
            {
                await _categoryService.DeleteAsync(id);
                return Ok(new EntityResponseMessage
                {
                    Message = "Category deleted successfully!",
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
        public async Task<IActionResult> Update(Guid id, [Bind("Id,CategoryName, Description")] Category category)
        {
            if (!ModelState.IsValid)
                return BadRequest(new EntityResponseMessage
                {
                    Message = "The data you entered is invalid.",
                    IsSuccess = false
                });

            if (id.Equals(category.Id))
            {
                try
                {
                    await _categoryService.UpdateAsync(id, category);
                    return Ok(new EntityResponseMessage
                    {
                        Message = "Category updated successfully!",
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

            return BadRequest(new EntityResponseMessage
            {
                Message = "Invalid ID",
                IsSuccess = false
            });
        }
    }
}