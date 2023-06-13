//using Ecommerce.ViewModels.Authentication.Users;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace Ecommerce.Data.Entities
//{
//    public class Message
//    {
//        [Key]
//        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//        public Guid Id { get; set; } = Guid.NewGuid();
//        [Required]
//        public Guid ChatRoomId { get; set; }
//        public ChatRoom ChatRoom { get; set; }
//        [Required]
//        public string Contents { get; set; }
//        [Required]
//        public string UserId { get; set; }
//        public ApplicationUser User { get; set; }
//        public DateTimeOffset PostedAt { get; set; } = DateTimeOffset.Now;
//        public bool Seen { get; set; } = false;
//    }
//}