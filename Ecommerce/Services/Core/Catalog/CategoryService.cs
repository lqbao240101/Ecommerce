using Ecommerce.Data.EF;
using Ecommerce.Data.Entities;
using Ecommerce.Services.Core.Common;
using Ecommerce.Services.Core.IService;

namespace Ecommerce.Services.Core.Catalog
{
    public class CategoryService : EntityBaseService<Category>, ICategoryService
    {
        public CategoryService(ApplicationDbContext context) : base(context)
        {
        }
    }
}