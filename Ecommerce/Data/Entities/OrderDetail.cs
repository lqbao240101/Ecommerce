namespace Ecommerce.Data.Entities
{
    public class OrderDetail
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public double PercentSale { get; set; }
        public decimal Total { get; set; }
    }
}