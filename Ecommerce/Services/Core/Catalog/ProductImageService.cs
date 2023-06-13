using Ecommerce.Data.EF;
using Ecommerce.Data.Entities;
using Ecommerce.Services.Core.IService;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Services.Core.Catalog
{
    public class ProductImageService : IProductImageService
    {
        private readonly ApplicationDbContext _context;

        public ProductImageService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task Create(Guid productId, string imageUrl)
        {
            var productImage = new ProductImage()
            {
                ProductId = productId,
                ImageUrl = imageUrl
            };

            await _context.ProductImages.AddAsync(productImage);
            await _context.SaveChangesAsync();
        }

        public async Task<List<string>> GetImages(Guid productId)
        {
            List<string> images = new();
            var listImage = await _context.ProductImages.Where(i => i.ProductId.Equals(productId)).ToListAsync();
            foreach (var image in listImage)
                images.Add(image.ImageUrl);
            return images;
        }

        public async Task<ProductImage> Check(string imageUrl)
        {
            var result = await _context.ProductImages.FirstOrDefaultAsync(n => n.ImageUrl.Equals(imageUrl));
            return result;
        }

        public async Task Delete(string imageUrl)
        {
            var productImage = await _context.ProductImages.FirstOrDefaultAsync(i => i.ImageUrl.Equals(imageUrl));
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Image.Equals(imageUrl));
            if (product is not null)
                product.Image = "";

            if (productImage is not null)
            {
                _context.ProductImages.Remove(productImage);
                await _context.SaveChangesAsync();
            }
        }
    }
}