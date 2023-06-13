using System.ComponentModel.DataAnnotations;

namespace Ecommerce.ViewModels.Reviews
{
    public class ReviewAddModel
    {
        public Guid ProductId { get; set; }
        public decimal Rating { get; set; }
        public string? Content { get; set; }
        public string CustomerId { get; set; }
    }
    public class ReviewUpdateModel
    {
        public decimal? Rating { get; set; }
        public string? Content { get; set; }
    }
}
