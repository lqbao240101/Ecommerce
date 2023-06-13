//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace Ecommerce.Data.Entities
//{
//    public class ChatRoom
//    {
//        [Key]
//        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//        public Guid Id { get; set; }
//        [Required]
//        public string AdminId { get; set; }
//        public ApplicationUser Admin { get; set; }
//        [Required]
//        public string CustomerId { get; set; }
//        public ApplicationUser Customer { get; set; }
//        public List<Message>? Messages { get; set; }
//    }
//}