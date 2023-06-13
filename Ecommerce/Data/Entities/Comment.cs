using Ecommerce.Services.Core.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Data.Entities
{
    public class Comment : IEntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        //Review
        [Required]
        public Guid ReviewId { get; set; }
        public Review Review { get; set; }
        //Customer
        [Required]
        public string CustomerId { get; set; }
        public ApplicationUser Customer { get; set; }
    }
}