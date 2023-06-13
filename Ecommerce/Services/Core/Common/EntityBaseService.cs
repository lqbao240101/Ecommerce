using AutoMapper;
using Ecommerce.Data.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace Ecommerce.Services.Core.Common
{
    public class EntityBaseService<T> : IEntityBaseService<T> where T : class, IEntityBase, new()
    {
        protected readonly ApplicationDbContext _context;
        protected readonly IMapper _mapper;

        public EntityBaseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public EntityBaseService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Set<T>().FirstOrDefaultAsync(x => x.Id.Equals(id)) ?? throw new KeyNotFoundException($"Not found with Id {id}");
            EntityEntry entityEntry = _context.Entry<T>(entity);
            entityEntry.State = EntityState.Deleted;

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();

        public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return await query.ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id) => await _context.Set<T>().FirstOrDefaultAsync(x => x.Id.Equals(id));
        public async Task<T> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return await query.FirstOrDefaultAsync(n => n.Id.Equals(id));
        }

        public async Task UpdateAsync(Guid id, T entity)
        {
            EntityEntry entityEntry = _context.Entry<T>(entity);
            entityEntry.State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }
    }
}