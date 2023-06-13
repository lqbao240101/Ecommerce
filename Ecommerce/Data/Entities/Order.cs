using Ecommerce.Data.Enums;
using Ecommerce.Services.Core.Common;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Data.Entities
{
    [Index(nameof(OrderId), IsUnique = true)]
    public class Order : IEntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string OrderId { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string Street { get; set; }
        public string Ward { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string CustomerId { get; set; }
        public ApplicationUser Customer { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }
}