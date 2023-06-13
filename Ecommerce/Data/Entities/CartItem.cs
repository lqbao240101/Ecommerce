using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Data.Entities
{
    public class CartItem
    {
        [Required]
        public string CustomerId { get; set; }
        public ApplicationUser Customer { get; set; }
        [Required]
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; } = 1;
    }
}