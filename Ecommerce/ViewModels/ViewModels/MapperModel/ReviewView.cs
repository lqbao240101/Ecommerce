using Ecommerce.ViewModels.ViewModels.MapperModel;

namespace Ecommerce.ViewModels.ViewModels.MapperModel
{
    public class ReviewView
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public decimal Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserCommentView Customer { get; set; }
        //Comment
        public List<CommentView>? Comments { get; set; }
    }
}