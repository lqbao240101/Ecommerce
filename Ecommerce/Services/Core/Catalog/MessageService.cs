//using Ecommerce.Data.EF;
//using Ecommerce.Data.Entities;
//using Ecommerce.Services.Core.IService;
//using Microsoft.EntityFrameworkCore;

//namespace Ecommerce.Services.Core.Catalog
//{
//    public class MessageService : IMessageService
//    {
//        private readonly ApplicationDbContext _context;
//        public MessageService(ApplicationDbContext context)
//        {
//            _context = context;
//        }
//        public async Task<List<Message>> GetMessagesForChatRoomAsync(Guid roomId)
//        {
//            var messages = await _context.Messages.Where(e => e.ChatRoomId.Equals(roomId)).ToListAsync<Message>();
//            return messages;
//        }
//        public async Task<bool> AddMessageToRoomAsync(Message message)
//        {
//            _context.Messages.Add(message);

//            var saveResults = await _context.SaveChangesAsync();

//            return saveResults > 0;
//        }
//    }
//}
