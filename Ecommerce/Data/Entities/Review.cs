using Ecommerce.Services.Core.Common;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Data.Entities
{
    public class Review : IEntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Content { get; set; } = "";
        [Required]
        [Column(TypeName = "decimal(18,4)")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        [RegularExpression(@"^[1-5](\.[05])?$", ErrorMessage = "Rating must be in increments of 0.5.")]
        public decimal Rating { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        //Customer
        [Required]
        public string CustomerId { get; set; }
        public ApplicationUser Customer { get; set; }
        //Product
        [Required]
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        //Comment
        public List<Comment>? Comments { get; set; }
    }
}