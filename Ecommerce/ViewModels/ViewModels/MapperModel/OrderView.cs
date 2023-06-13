using Ecommerce.Data.Enums;
using Ecommerce.ViewModels.ViewModels.MapperModel;
using Newtonsoft.Json;

namespace Ecommerce.ViewModels.ViewModels.MapperModel
{
    public class OrderView
    {
        public string? OrderId { get; set; }
        public DateTime? DateCreated { get; set; }
        public OrderStatus Status { get; set; } = 0;
        public string Street { get; set; }
        public string Ward { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        [JsonIgnore]
        public string? CustomerId { get; set; }
        public List<OrderDetailView>? OrderDetails { get; set; }
    }
}