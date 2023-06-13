using Ecommerce.Data.Entities;

namespace Ecommerce.ViewModels.ViewModels.MapperModel
{
    public class ProductGetAllView
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public double PercentSale { get; set; }
        public string? Image { get; set; }
        public int Quantity { get; set; }
        public Guid CategoryId { get; set; }
        public decimal Rating { get; set; }
        public Category Category { get; set; }
    }
}
