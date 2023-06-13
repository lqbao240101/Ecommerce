namespace Ecommerce.ViewModels.Products
{
    public class ProductAddModel
    {
        public string ProductName { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public double PercentSale { get; set; }
        public int Quantity { get; set; }
        public IFormFile? File { get; set; }
        public Guid CategoryId { get; set; }
    }

    public class ProductUpdateModel
    {
        public string? ProductName { get; set; }

        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public double? PercentSale { get; set; }
        public int? Quantity { get; set; }
        public Guid? CategoryId { get; set; }
    }
}
