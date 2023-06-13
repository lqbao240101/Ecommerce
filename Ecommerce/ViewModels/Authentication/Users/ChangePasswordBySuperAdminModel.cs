using System.ComponentModel.DataAnnotations;

namespace Ecommerce.ViewModels.Authentication.Users
{
    public class ChangePasswordBySuperAdminModel
    {
        [Required]
        public string NewPassword { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
    }
}