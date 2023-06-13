using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Ecommerce.Services.Core.Common;

namespace Ecommerce.Data.Entities
{
    public class Token : IEntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
