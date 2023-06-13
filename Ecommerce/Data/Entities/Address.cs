using Ecommerce.Services.Core.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Data.Entities
{
    public class Address : IEntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Street { get; set; }
        [Required]
        public string Ward { get; set; }
        [Required]
        public string District { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string CustomerId { get; set; }
        public ApplicationUser Customer { get; set; }
    }
}