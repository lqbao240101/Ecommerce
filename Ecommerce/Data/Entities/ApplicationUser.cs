using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(50)]
        [StringLength(30, MinimumLength = 2)]
        public string FullName { get; set; }
        public string? Avatar { get; set; }
        public string? FolderId { get; set; }
        public List<Address>? Addresses { get; set; }
        public List<Order>? Orders { get; set; }
        public List<Review>? Reviews { get; set; }
        public List<Comment>? Comments { get; set; }
    }
}