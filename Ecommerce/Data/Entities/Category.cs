using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Ecommerce.Services.Core.Common;

namespace Ecommerce.Data.Entities
{
    [Table("Categories")]
    public class Category : IEntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Tên không được để trống")]
        [MinLength(2, ErrorMessage = "Tên phải có ít nhất 2 ký tự")]
        public string CategoryName { get; set; }

        [Required(ErrorMessage = "Thông tin không được để trống")]
        [MinLength(10, ErrorMessage = "Thông tin phải có ít nhất 10 ký tự")]
        [Column(TypeName = "ntext")]
        public string Description { get; set; }

        // Một category có nhiều products
        public List<Product>? Products { get; set; }
    }
}