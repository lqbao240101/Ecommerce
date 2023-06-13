using AutoMapper;
using AutoMapper.QueryableExtensions;
using Ecommerce.Data.EF;
using Ecommerce.Data.Entities;
using Ecommerce.Services.Core.IService;
using Ecommerce.ViewModels.Common;
using Ecommerce.ViewModels.EntityParameters;
using Ecommerce.ViewModels.Reviews;
using Ecommerce.ViewModels.ViewModels.MapperModel;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Services.Core.Catalog
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public ReviewService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Review> CreateAsync(string userId, ReviewAddModel data)
        {
            var product = await _context.Products.FirstOrDefaultAsync(e => e.Id.Equals(data.ProductId)) ?? throw new KeyNotFoundException($"Not found product with id {data.ProductId}");
            data.CustomerId = userId;
            var review = _mapper.Map<ReviewAddModel, Review>(data);

            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();

            await UpdateRatingProduct(data.ProductId);
            return review;
        }

        public async Task<Review> UpdateAsync(string userId, ReviewUpdateModel data, Guid id)
        {
            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id.Equals(id) && r.CustomerId.Equals(userId)) ?? throw new KeyNotFoundException($"Review with Id {id} is not found");

            review = _mapper.Map(data, review);
            _context.Update(review);
            await _context.SaveChangesAsync();

            await UpdateRatingProduct(review.ProductId);
            return review;
        }

        public async Task<Review?> CheckReview(string userId, Guid Id)
        {
            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id.Equals(Id) && r.CustomerId.Equals(userId));
            return review;
        }

        public async Task DeleteAsync(string userId, Guid Id)
        {
            var review = await _context.Reviews.FirstOrDefaultAsync(n => n.CustomerId.Equals(userId) && n.Id.Equals(Id));
            if (review is not null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
                await UpdateRatingProduct(review.ProductId);
            }
            else
                throw new KeyNotFoundException($"Review with Id {Id} is not found");
        }

        public async Task UpdateRatingProduct(Guid productId)
        {
            decimal totalRating = 0;
            decimal rating = 0;

            var reviews = await _context.Reviews.Where(n => n.ProductId.Equals(productId)).ToListAsync();

            if (reviews is not null)
            {
                foreach (var r in reviews)
                    totalRating += r.Rating;

                rating = totalRating / reviews.Count;
            }

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id.Equals(productId));

            product.Rating = rating;
            await _context.SaveChangesAsync();
        }

        public PagedList<ReviewView> GetReviewsWithPaging(Guid productId, ReviewParameters reviewParameters)
        {
            var reviews = _context.Reviews.Include(r => r.Comments).Include(r => r.Customer).Where(n => n.ProductId.Equals(productId));
            var reviewViews = reviews.ProjectTo<ReviewView>(_mapper.ConfigurationProvider);
            return PagedList<ReviewView>.ToPagedList(reviewViews, reviewParameters.PageNumber, reviewParameters.PageSize);
        }
    }
}