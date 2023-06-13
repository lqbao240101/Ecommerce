using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Data.Entities
{
    public class ProductImage
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public string ImageUrl { get; set; }
    }
}