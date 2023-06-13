using Ecommerce.Data.Static;
using Ecommerce.Services.Core.IService;
using Ecommerce.ViewModels.Common;
using Ecommerce.ViewModels.EntityParameters;
using Ecommerce.ViewModels.Products;
using Ecommerce.ViewModels.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Ecommerce.Application.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Authorize(Roles = UserRoles.SuperAdmin + "," + UserRoles.Admin)]
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IDriveService _driveService;
        private readonly IProductImageService _productImageService;
        public ProductController(IProductService productService, IDriveService driveService, IProductImageService productImageService)
        {
            _productService = productService;
            _driveService = driveService;
            _productImageService = productImageService;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetAll([FromQuery] ProductParameters parameters)
        {
            var products = _productService.GetProductsWithPaging(parameters);

            var metadata = new
            {
                products.TotalCount,
                products.PageSize,
                products.CurrentPage,
                products.TotalPages,
                products.HasNext,
                products.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            return Ok(new EntityResponseMessage
            {
                Data = products,
                IsSuccess = true
            });
        }

        [AllowAnonymous]
        [HttpGet("Images/{id}")]
        public async Task<IActionResult> GetImages(Guid id)
        {
            var list = await _productImageService.GetImages(id);

            if (list != null)
                return Ok(new EntityResponseMessage
                {
                    Data = list,
                    IsSuccess = true
                });

            return NotFound(new EntityResponseMessage
            {
                Message = $"Product with ID {id} not found.",
                IsSuccess = false
            });
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> Detail(Guid id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product is null)
                return NotFound(new EntityResponseMessage
                {
                    Message = $"Product with ID {id} not found.",
                    IsSuccess = false
                });

            return Ok(new EntityResponseMessage
            {
                Data = product,
                IsSuccess = true
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductAddModel product)
        {
            if (!ModelState.IsValid)
                return BadRequest(new EntityResponseMessage
                {
                    Message = "The data you entered is invalid.",
                    IsSuccess = false
                });

            string folderId = _driveService.AddFolder(product.ProductName, "1oHnUhNG0Nqzahq7q5It-dHwCC8dHDWMJ");
            string imageUrl = "";

            try
            {
                if (product.File != null)
                {
                    string[] s = product.File.FileName.Split('.');
                    string ex = "." + s[s.Length - 1];
                    var filePath = "DriveImage/download" + ex;
                    if (product.File.Length > 0)
                    {
                        using var stream = System.IO.File.Create(filePath);
                        await product.File.CopyToAsync(stream);
                    }

                    imageUrl = _driveService.AddFile(filePath, ex, folderId);

                    var result = await _productService.AddNewProductAsync(product, folderId, imageUrl);
                    await _productImageService.Create(result.Id, imageUrl);
                }
                else
                    await _productService.AddNewProductAsync(product, folderId, imageUrl);

                return Ok(new EntityResponseMessage
                {
                    Message = "Product created successfully!",
                    IsSuccess = true
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

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(Guid id, ProductUpdateModel product)
        {
            if (!ModelState.IsValid)
                return BadRequest(new EntityResponseMessage
                {
                    Message = "The data you entered is invalid.",
                    IsSuccess = false
                });

            try
            {
                var x = await _productService.GetByIdAsync(id);
                if (x is not null)
                {
                    if (product.ProductName != null && !x.ProductName.Equals(product.ProductName))
                    {
                        await _driveService.RenamingFolder(product.ProductName, x.FolderUrl);
                    }
                    await _productService.UpdateProductAsync(product, id);
                    return Ok(new EntityResponseMessage
                    {
                        Message = "Product updated successfully!",
                        IsSuccess = true
                    });
                }

                return NotFound(new EntityResponseMessage
                {
                    Message = $"Product with ID {id} not found.",
                    IsSuccess = false
                });
            }
            catch (KeyNotFoundException n)
            {
                return NotFound(new EntityResponseMessage
                {
                    Message = n.Message,
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product is null)
                return NotFound(new EntityResponseMessage
                {
                    Message = $"Product with ID {id} not found.",
                    IsSuccess = false
                });

            var result = await _driveService.Remove(product.FolderUrl);

            if (result)
            {
                try
                {
                    await _productService.DeleteAsync(id);
                    return Ok(new EntityResponseMessage
                    {
                        Message = "Product deleted successfully!",
                        IsSuccess = true
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

            return StatusCode(500, new EntityResponseMessage
            {
                Message = "Something wrong with drive",
                IsSuccess = false
            });
        }

        [HttpPost("/Image")]
        public async Task<IActionResult> AddImage([FromForm] NewProductImageModel data)
        {
            if (!ModelState.IsValid)
                return BadRequest(new EntityResponseMessage
                {
                    Message = "The data you entered is invalid.",
                    IsSuccess = false
                });

            var product = await _productService.GetByIdAsync(data.ProductId);
            if (product is null)
                return NotFound(new EntityResponseMessage
                {
                    Message = $"Product with ID {data.ProductId} not found.",
                    IsSuccess = false
                });

            try
            {
                IFormFile file = data.Image;
                string[] s = file.FileName.Split('.');
                string ex = "." + s[s.Length - 1];
                var filePath = "DriveImage/download" + ex;
                if (file.Length > 0)
                {
                    using var stream = System.IO.File.Create(filePath);
                    await file.CopyToAsync(stream);
                }

                string imageUrl = _driveService.AddFile(filePath, ex, product.FolderUrl);

                await _productImageService.Create(data.ProductId, imageUrl);
                return Ok(new EntityResponseMessage
                {
                    Message = "Image added successfully!",
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

        [HttpPatch("/Image/{productId}")]
        public async Task<IActionResult> UpdateFieldImage(Guid productId, [FromForm] UpdateFieldImageModel data)
        {
            var product = await _productService.GetByIdAsync(productId);
            if (product is null)
                return NotFound(new EntityResponseMessage
                {
                    Message = $"Product with ID {productId} not found.",
                    IsSuccess = false
                });

            if (data.Image != null && string.IsNullOrEmpty(data.ImageUrl))
            {
                IFormFile file = data.Image;
                string[] s = file.FileName.Split('.');
                string ex = "." + s[s.Length - 1];
                var filePath = "DriveImage/download" + ex;
                if (file.Length > 0)
                {
                    using var stream = System.IO.File.Create(filePath);
                    await file.CopyToAsync(stream);
                }
                string imageUrl = _driveService.AddFile(filePath, ex, product.FolderUrl);
                await _productService.UpdateFieldImageAsync(productId, imageUrl);
                await _productImageService.Create(productId, imageUrl);
                return Ok(new EntityResponseMessage
                {
                    Data = imageUrl,
                    Message = "Product Image updated successfully!",
                    IsSuccess = true
                });
            }

            if (!string.IsNullOrEmpty(data.ImageUrl) && data.Image == null)
            {
                string url = Uri.UnescapeDataString(data.ImageUrl).ToString();
                await _productService.UpdateFieldImageAsync(productId, url);
                return Ok(new EntityResponseMessage
                {
                    Data = url,
                    Message = "Product Image updated successfully!",
                    IsSuccess = true
                });
            }

            return BadRequest(new EntityResponseMessage
            {
                Message = "The data you entered is invalid.",
                IsSuccess = false
            });
        }

        [HttpDelete("/Image")]
        public async Task<IActionResult> DeleteImage(string url)
        {
            if (string.IsNullOrEmpty(url))
                return BadRequest(new EntityResponseMessage
                {
                    Message = "The data you entered is invalid.",
                    IsSuccess = false
                });

            string imageUrl = Uri.UnescapeDataString(url).ToString();
            string id = imageUrl.Split(new[] { "id=" }, StringSplitOptions.None)[1];

            var image = await _productImageService.Check(imageUrl);

            if (image is null)
                return NotFound(new EntityResponseMessage
                {
                    Message = "Product Image Url doesn't exist.",
                    IsSuccess = false
                });

            var result = await _driveService.Remove(id);
            if (result)
            {
                await _productImageService.Delete(imageUrl);
                return Ok(new EntityResponseMessage
                {
                    Message = "Product Image deleted successfully!",
                    IsSuccess = true
                });
            }

            return StatusCode(500, new EntityResponseMessage
            {
                Message = "Something wrong",
                IsSuccess = false
            });
        }

        [AllowAnonymous]
        [HttpGet("/Search/{searchString}")]
        public async Task<IActionResult> Search(string searchString)
        {
            var result = await _productService.Search(searchString);
            return Ok(new EntityResponseMessage
            {
                Data = result,
                IsSuccess = true
            });
        }
    }
}