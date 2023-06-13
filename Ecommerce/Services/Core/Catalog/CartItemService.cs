using AutoMapper;
using Ecommerce.Data.EF;
using Ecommerce.Data.Entities;
using Ecommerce.Services.Core.IService;
using Ecommerce.ViewModels.CartItem;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Services.Core.Catalog
{
    public class CartItemService : ICartItemService
    {
        protected readonly ApplicationDbContext _context;
        protected readonly IMapper _mapper;
        public CartItemService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<CartItem> AddNewCartItemAsync(string userId, CartItemAddModel data)
        {
            var currentCartItem = await _context.CartItems.FirstOrDefaultAsync(n => n.CustomerId.Equals(userId) && n.ProductId.Equals(data.ProductId));

            if (currentCartItem is null)
            {

                data.CustomerId = userId;
                currentCartItem = _mapper.Map<CartItemAddModel, CartItem>(data);

                await _context.CartItems.AddAsync(currentCartItem);
            }
            else
                currentCartItem.Quantity += (int)data.Quantity;

            await _context.SaveChangesAsync();
            return currentCartItem;
        }

        public async Task DeleteCartItemAsync(string userId, Guid productId)
        {
            var cartItem = await _context.CartItems.FirstOrDefaultAsync(n => n.CustomerId.Equals(userId) && n.ProductId.Equals(productId)) ?? throw new KeyNotFoundException($"Cart item is not found with product Id {productId}");
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task<CartItem> GetCartItemDetail(string userId, Guid productId)
        {
            var cartItem = await _context.CartItems.Include(n => n.Product).FirstOrDefaultAsync(n => n.ProductId.Equals(productId) && n.CustomerId.Equals(userId));
            return cartItem;
        }

        public async Task<List<CartItem>> GetCartItemsByUserId(string userId)
        {
            var cartItem = await _context.CartItems.Include(n => n.Product).Where(n => n.CustomerId.Equals(userId)).ToListAsync();
            return cartItem;
        }

        public async Task<CartItem> UpdateCartItemAsync(CartItemUpdateModel data, string userId, Guid productId)
        {
            var cartItem = await _context.CartItems.FirstOrDefaultAsync(n => n.CustomerId.Equals(userId) && n.ProductId.Equals(productId)) ?? throw new KeyNotFoundException($"Cart item is not found with product Id {productId}");
            cartItem.Quantity = data.Quantity;
            await _context.SaveChangesAsync();
            return cartItem;
        }
    }
}