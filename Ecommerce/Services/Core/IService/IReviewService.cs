using Ecommerce.Data.Entities;
using Ecommerce.ViewModels.Common;
using Ecommerce.ViewModels.EntityParameters;
using Ecommerce.ViewModels.Reviews;
using Ecommerce.ViewModels.ViewModels;
using Ecommerce.ViewModels.ViewModels.MapperModel;

namespace Ecommerce.Services.Core.IService
{
    public interface IReviewService
    {
        Task<Review> CreateAsync(string userId, ReviewAddModel data);
        PagedList<ReviewView> GetReviewsWithPaging(Guid productId, ReviewParameters reviewParameters);
        Task<Review> UpdateAsync(string userId, ReviewUpdateModel data, Guid id);
        Task<Review?> CheckReview(string userId, Guid Id);
        Task DeleteAsync(string userId, Guid Id);
    }
}