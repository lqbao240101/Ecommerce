using Ecommerce.Data.Entities;
using Ecommerce.Services.Core.Common;
using Ecommerce.ViewModels.Common;
using Ecommerce.ViewModels.EntityParameters;
using Ecommerce.ViewModels.Products;
using Ecommerce.ViewModels.ViewModels.MapperModel;

namespace Ecommerce.Services.Core.IService
{
    public interface IProductService : IEntityBaseService<Product>
    {
        PagedList<ProductGetAllView> GetProductsWithPaging(ProductParameters productParameters);
        Task<Product> AddNewProductAsync(ProductAddModel data, string folderUrl, string image);
        Task<Product> UpdateProductAsync(ProductUpdateModel data, Guid id);
        Task UpdateFieldImageAsync(Guid id, string imageUrl);
        Task<List<Product>> Search(string searchString);
    }
}