using AutoMapper;
using AutoMapper.QueryableExtensions;
using Ecommerce.Data.EF;
using Ecommerce.Data.Entities;
using Ecommerce.Services.Core.Common;
using Ecommerce.Services.Core.IService;
using Ecommerce.ViewModels.Common;
using Ecommerce.ViewModels.EntityParameters;
using Ecommerce.ViewModels.Products;
using Ecommerce.ViewModels.ViewModels.MapperModel;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Services.Core.Catalog
{
    public class ProductService : EntityBaseService<Product>, IProductService
    {
        public ProductService(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {

        }

        public async Task<Product> AddNewProductAsync(ProductAddModel data, string folderUrl, string image)
        {
            var newProduct = _mapper.Map<ProductAddModel, Product>(data);

            await _context.Products.AddAsync(newProduct);
            await _context.SaveChangesAsync();
            return newProduct;
        }

        public async Task<Product> UpdateProductAsync(ProductUpdateModel data, Guid id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(n => n.Id.Equals(id)) ?? throw new KeyNotFoundException($"Product with ID {id} not found.");

            product = _mapper.Map(data, product);
            _context.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task UpdateFieldImageAsync(Guid id, string imageUrl)
        {
            var product = await _context.Products.FirstOrDefaultAsync(n => n.Id.Equals(id)) ?? throw new KeyNotFoundException($"Product with ID {id} not found.");
            product.Image = imageUrl;
            await _context.SaveChangesAsync();
        }

        public PagedList<ProductGetAllView> GetProductsWithPaging(ProductParameters productParameters)
        {
            var products = _context.Products.Include(o => o.Category);
            var productViews = products.ProjectTo<ProductGetAllView>(_mapper.ConfigurationProvider);
            return PagedList<ProductGetAllView>.ToPagedList(productViews, productParameters.PageNumber, productParameters.PageSize);
        }

        public async Task<List<Product>> Search(string searchString)
        {
            var a = searchString.Trim();
            string[] b = a.Split(' ');
            for (int i = 0; i < b.Length; i++)
            {
                if (i == 0)
                    b[i] = "%" + b[i] + "%";
                b[i] += "%";
            }

            string query = string.Join(" ", b);
            var products = await _context.Products
                .Where(p => EF.Functions.Like(EF.Functions.Collate(p.ProductName, "Latin1_General_CI_AI"), query)).ToListAsync();
            return products;
        }
    }
}