namespace Ecommerce.ViewModels.ViewModels.MapperModel
{
    public class OrderDetailView
    {
        public ProductOrderView Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public double PercentSale { get; set; }
        public decimal Total { get; set; }
    }
}