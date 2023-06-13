using Ecommerce.Data.Entities;

namespace Ecommerce.Services.Core.IService
{
    public interface IProductImageService
    {
        Task Create(Guid productId, string imageUrl);
        Task<List<string>> GetImages(Guid productId);
        Task<ProductImage> Check(string imageUrl);
        Task Delete(string imageUrl);
    }
}