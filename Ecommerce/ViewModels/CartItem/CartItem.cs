using Newtonsoft.Json;

namespace Ecommerce.ViewModels.CartItem
{
    public class CartItemAddModel
    {
        public Guid ProductId { get; set; }
        public int? Quantity { get; set; } = 1;
        [System.Text.Json.Serialization.JsonIgnore] 
        public string? CustomerId { get; set; }
    }
    public class CartItemUpdateModel
    {
        public int Quantity { get; set; }
    }
}
