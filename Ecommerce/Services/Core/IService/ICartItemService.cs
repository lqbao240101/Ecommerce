using Ecommerce.Data.Entities;
using Ecommerce.ViewModels.CartItem;

namespace Ecommerce.Services.Core.IService
{
    public interface ICartItemService
    {
        Task<List<CartItem>> GetCartItemsByUserId(string userId);

        Task<CartItem> GetCartItemDetail(string userId, Guid productId);

        Task<CartItem> AddNewCartItemAsync(string userId, CartItemAddModel data);
        Task<CartItem> UpdateCartItemAsync(CartItemUpdateModel data, string userId, Guid productId);
        Task DeleteCartItemAsync(string userId, Guid productId);
    }
}