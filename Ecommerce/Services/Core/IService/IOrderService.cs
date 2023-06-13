using Ecommerce.Data.Entities;
using Ecommerce.ViewModels.ViewModels.MapperModel;

namespace Ecommerce.Services.Core.IService
{
    public interface IOrderService
    {
        Task CheckBeforeCreateOrderAsync(List<Guid> productIds, OrderPost orderView, string userId);
        Task CreateOrder(Dictionary<CartItem, Product> list, OrderPost orderView, string userId);
        Task<List<OrderView>> GetOrdersByUser(string userId);
        Task<OrderView> GetOrderByUser(string userId, Guid orderId);
        Task CancelOrder(Guid orderId, string role, string userId);
        Task ConfirmOrder(Guid orderId);
        Task<List<OrderView>> GetOrdersByAdmin();
        Task<string> GenerateOrderId();
    }
}