using AutoMapper;
using Ecommerce.Data.EF;
using Ecommerce.Data.Entities;
using Ecommerce.Services.Core.IService;
using Ecommerce.ViewModels.Comments;
using Ecommerce.ViewModels.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Services.Core.Catalog
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext _context;
        protected IMapper _mapper;
        public CommentService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Comment> CreateAsync(string userId, CommentAddModel data)
        {
            data.CustomerId = userId;

            var item = _mapper.Map<CommentAddModel, Comment>(data);

            await _context.Comments.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }
        public async Task<Comment> CheckComment(string userId, Guid Id)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(r => r.Id.Equals(Id) && r.CustomerId.Equals(userId));
#pragma warning disable CS8603 // Possible null reference return.
            return comment;
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task DeleteAsync(string userId, Guid Id)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(n => n.CustomerId.Equals(userId) && n.Id.Equals(Id)) ?? throw new KeyNotFoundException($"Comment with Id {Id} is not found.");
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<Comment> UpdateAsync(string userId, CommentUpdateModel data, Guid id)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(r => r.Id.Equals(id) && r.CustomerId.Equals(userId)) ?? throw new KeyNotFoundException($"Comment with Id {id} is not found.");
            comment = _mapper.Map(data, comment);
            _context.Update(comment);
            await _context.SaveChangesAsync();
            return comment;
        }
    }
}