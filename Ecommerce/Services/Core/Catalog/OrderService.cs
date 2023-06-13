using AutoMapper;
using Ecommerce.Data.EF;
using Ecommerce.Data.Entities;
using Ecommerce.Data.Enums;
using Ecommerce.Data.Static;
using Ecommerce.Services.Core.IService;
using Ecommerce.ViewModels.ViewModels.MapperModel;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Services.Core.Catalog
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public OrderService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        //public async Task CheckBeforeCreateOrderAsync(List<int> productIds, string userId, string street, string ward, string district, string city)
        public async Task CheckBeforeCreateOrderAsync(List<Guid> productIds, OrderPost orderPost, string userId)
        {
            Dictionary<CartItem, Product> list = new();

            foreach (var productId in productIds)
            {
                var cartItem = await _context.CartItems.FirstOrDefaultAsync(n => n.ProductId.Equals(productId) && n.CustomerId.Equals(userId)) ?? throw new KeyNotFoundException($"Không tồn tại sản phẩm có id {productId} trong giỏ hàng"); ;

                var product = await _context.Products.FirstOrDefaultAsync(n => n.Id.Equals(cartItem.ProductId)) ?? throw new KeyNotFoundException($"Sản phẩm có id {productId} không tồn tại");

                if (cartItem.Quantity <= product.Quantity)
                    list.Add(cartItem, product);
                else
                    throw new ArgumentException($"Sản phẩm {product.ProductName} không đủ số lượng yêu cầu");
            }

            await CreateOrder(list, orderPost, userId);
        }
        // public async Task CreateOrder(Dictionary<CartItem, Product> list, string userId, string street, string ward, string district, string city)
        public async Task CreateOrder(Dictionary<CartItem, Product> list, OrderPost orderPost, string userId)
        {
            var orderId = await GenerateOrderId();

            Order order = _mapper.Map<Order>(orderPost);
            order.OrderId = orderId;
            order.CustomerId = userId;


            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            foreach (var cartItem in list)
            {
                var orderDetail = new OrderDetail()
                {
                    OrderId = order.Id,
                    ProductId = cartItem.Key.ProductId,
                    Quantity = cartItem.Key.Quantity,
                    PercentSale = cartItem.Value.PercentSale,
                    Price = cartItem.Value.Price,
                    Total = cartItem.Key.Quantity * cartItem.Value.Price * (decimal)(100.0 - cartItem.Value.PercentSale) / 100
                };

                cartItem.Value.Quantity -= orderDetail.Quantity;

                await _context.OrderDetails.AddAsync(orderDetail);
            }

            _context.CartItems.RemoveRange(list.Keys);
            await _context.SaveChangesAsync();
        }
        public async Task<List<OrderView>> GetOrdersByAdmin()
        {
            var orders = _mapper.Map<List<OrderView>>(await _context.Orders.Include(n => n.OrderDetails).ToListAsync());
            return orders;
        }
        public async Task<List<OrderView>> GetOrdersByUser(string userId)
        {
            var orders = _mapper.Map<List<OrderView>>(await _context.Orders.Include(n => n.OrderDetails).ThenInclude(n => n.Product).Where(o => o.CustomerId.Equals(userId)).ToListAsync());
            return orders;
        }

        public async Task<OrderView> GetOrderByUser(string userId, Guid orderId)
        {
            var order = _mapper.Map<OrderView>(await _context.Orders.Include(n => n.OrderDetails)
                .FirstOrDefaultAsync(o => o.CustomerId.Equals(userId) && o.Id.Equals(orderId)));
            return order;
        }

        public async Task CancelOrder(Guid id, string role, string userId)
        {
            Order? order = new();
            if (role.Equals(UserRoles.Admin) || role.Equals(UserRoles.SuperAdmin))
                order = await _context.Orders.FirstOrDefaultAsync(n => n.Id.Equals(id));
            else if (role.Equals(UserRoles.User))
                order = await _context.Orders.FirstOrDefaultAsync(n => n.Id.Equals(id) && n.CustomerId.Equals(userId));

            if (order is not null)
            {
                if (order.Status.Equals(OrderStatus.Pending))
                {
                    order.Status = OrderStatus.Canceled;

                    var listOrderDetail = await _context.OrderDetails.Where(o => o.OrderId.Equals(id)).ToListAsync();

                    foreach (var orderDetail in listOrderDetail)
                    {
                        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id.Equals(orderDetail.ProductId));

                        if (product is not null)
                            product.Quantity += orderDetail.Quantity;
                    }

                    await _context.SaveChangesAsync();
                }
                throw new ArgumentException($"Hủy đơn hàng không thành công do trạng thái không hợp lệ");
            }

            throw new KeyNotFoundException($"Không tìm thấy đơn hàng có mã {id}");
        }

        public async Task ConfirmOrder(Guid id)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(p => p.Id.Equals(id)) ?? throw new KeyNotFoundException($"Không tìm thấy đơn hàng có mã {id}");

            order.Status = OrderStatus.Confirmed;
            await _context.SaveChangesAsync();
        }

        public async Task<string> GenerateOrderId()
        {
            string day = DateTime.Today.ToString("ddMMyyyy");
            string orderId = day + "-";
            var order = await _context.Orders
                .Where(n => n.DateCreated.Date == DateTime.Now.Date)
                .OrderByDescending(n => n.DateCreated)
                .FirstOrDefaultAsync();
            int number = 100;

            if (order is not null)
            {
                string x = order.OrderId;
                string[] parts = x.Split('-');
                string y = parts[1];
                number = int.Parse(y) + 1;
            }

            orderId += number;

            return orderId;
        }
    }
}