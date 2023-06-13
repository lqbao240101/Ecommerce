namespace Ecommerce.ViewModels.Common
{
    public class EntityResponseMessage
    {
        public object? Data { get; set; }
        public string? Message { get; set; }
        public bool IsSuccess { get; set; }
    }
}