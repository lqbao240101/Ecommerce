using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Ecommerce.Services.Core.Common;
using Microsoft.AspNetCore.Authorization;
using Ecommerce.Data.Static;
using HotChocolate.Authorization;

namespace Ecommerce.Data.Entities
{
    [Table("Products")]
    public class Product : IEntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string ProductName { get; set; }
        public string? Description { get; set; } = "";
        public decimal Price { get; set; } = 0;
        public double PercentSale { get; set; } = 0;
        public string? Image { get; set; }
        public string FolderUrl { get; set; }
        public int Quantity { get; set; } = 0;
        public decimal Rating { get; set; } = 0;
        [Required]
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
        public List<Review>? Reviews { get; set; }
    }
}