using System.ComponentModel.DataAnnotations;

namespace Ecommerce.ViewModels.Authentication.Users
{
    public class RegisterAdminViewModel
    {
        [Required(ErrorMessage = "Họ tên không được để trống")]
        public string FullName { get; set; }
        [Required]
        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Password { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string ConfirmPassword { get; set; }
    }
}
