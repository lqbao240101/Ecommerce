using Ecommerce.Data.Entities;
using Ecommerce.ViewModels.Comments;
using Ecommerce.ViewModels.ViewModels;

namespace Ecommerce.Services.Core.IService
{
    public interface ICommentService
    {
        Task<Comment> CreateAsync(string userId, CommentAddModel data);
        Task<Comment> UpdateAsync(string userId, CommentUpdateModel data, Guid id);
        Task<Comment> CheckComment(string userId, Guid Id);
        Task DeleteAsync(string userId, Guid Id);
    }
}