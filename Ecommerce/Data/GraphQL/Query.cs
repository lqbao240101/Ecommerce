using Ecommerce.Data.EF;
using Ecommerce.Data.Entities;

namespace Ecommerce.Data.GraphQL
{
    public class Query
    {
        [UsePaging(IncludeTotalCount = true, DefaultPageSize = 3)]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Product> GetProducts([Service] ApplicationDbContext context)
        {
            return context.Products;
        }
    }
}
