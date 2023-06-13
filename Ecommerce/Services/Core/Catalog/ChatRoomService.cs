//using Ecommerce.Data.EF;
//using Ecommerce.Data.Entities;
//using Ecommerce.Data.Static;
//using Ecommerce.Services.Core.IService;
//using Microsoft.EntityFrameworkCore;

//namespace Ecommerce.Services.Core.Catalog
//{
//    public class ChatRoomService : IChatRoomService
//    {
//        private readonly ApplicationDbContext _context;
//        public ChatRoomService(ApplicationDbContext context)
//        {
//            _context = context;
//        }
//        public async Task<ChatRoom?> CheckChatRoomAsync(string adminId, string customerId)
//        {
//            var chatRoom = await _context.ChatRooms.FirstOrDefaultAsync(e => e.CustomerId.Equals(customerId) && e.AdminId.Equals(adminId));
//            return chatRoom;
//        }
//        public async Task<List<ChatRoom>> AdminGetAllAsync(string adminId)
//        {
//            var chatRooms = await _context.ChatRooms.Where(e => e.AdminId.Equals(adminId)).ToListAsync();
//            return chatRooms;
//        }

//        public async Task<List<ChatRoom>> CustomerGetAllAsync(string customerId)
//        {
//            var chatRooms = await _context.ChatRooms.Where(e => e.CustomerId.Equals(customerId)).ToListAsync();
//            return chatRooms;
//        }
//        public async Task<bool> AddChatRoomAsync(ChatRoom chatRoom)
//        {
//            await _context.ChatRooms.AddAsync(chatRoom);

//            var saveResults = await _context.SaveChangesAsync();

//            return saveResults > 0;
//        }
//        public async Task<ChatRoom?> GetOneAsync(string userId, string findId, string userRole)
//        {
//            if (userRole.Equals(UserRoles.User))
//                return await CheckChatRoomAsync(findId, userId);
//            return await CheckChatRoomAsync(userId, findId);
//        }
//    }
//}
