namespace Ecommerce.ViewModels.ViewModels
{
    public class NewProductImageModel
    {
        public Guid ProductId { get; set; }
        public IFormFile Image { get; set; }
    }
}
