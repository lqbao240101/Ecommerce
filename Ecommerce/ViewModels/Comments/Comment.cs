using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.ViewModels.Comments
{
    public class CommentAddModel
    {
        public string Content { get; set; }
        public Guid ReviewId { get; set; }
        [JsonIgnore]
        public string CustomerId { get; set; }
    }

    public class CommentUpdateModel
    {
        public string Content { get; set; }
    }
}
