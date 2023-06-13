namespace Ecommerce.ViewModels.ViewModels.MapperModel
{
    public class CommentView
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserCommentView Customer { get; set; }
    }
}