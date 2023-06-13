//using Ecommerce.Data.Entities;
//using Ecommerce.Data.Static;
//using Ecommerce.Services.Core.IService;
//using Microsoft.AspNetCore.SignalR;
//using System.Runtime.CompilerServices;
//using System.Security.Claims;
//using Twilio.TwiML.Voice;

//namespace Ecommerce.Hubs
//{
//    public class ChatHub : Hub
//    {
//        private readonly IChatRoomService _chatRoomService;
//        private readonly IMessageService _messageService;
//        public bool UserOnline;

//        public ChatHub(IChatRoomService chatRoomService, IMessageService messageService)
//        { 
//            _chatRoomService = chatRoomService;
//            _messageService = messageService;
//        }
//        public async System.Threading.Tasks.Task SendMessage(Guid chatRoomId, string userId, string message)
//        {
//            var m = new Message()
//            {
//                ChatRoomId = chatRoomId,
//                Contents = message,
//                UserId = userId
//            };
//            await _messageService.AddMessageToRoomAsync(m);
//            await Clients.All.SendAsync("ReceiveMessage", userId, message, chatRoomId, m.Id, m.PostedAt);
//        }
//        public async System.Threading.Tasks.Task AddChatRoom(string userId, string userRole, string partNerId)
//        {
//            var customerId = userRole.Equals(UserRoles.User) ? userId : partNerId;
//            var adminId = userRole.Equals(UserRoles.User) ? partNerId : userId;
//            var chatRoom = new ChatRoom
//            {
//                CustomerId = customerId,
//                AdminId = adminId
//            };
//            await _chatRoomService.AddChatRoomAsync(chatRoom);
//            await Clients.All.SendAsync("NewRoom", partNerId, chatRoom.Id);
//        }

//        public override async System.Threading.Tasks.Task OnConnectedAsync()
//        {
//            UserOnline = true;
//            await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
//            await base.OnConnectedAsync();
//        }

//        public override async System.Threading.Tasks.Task OnDisconnectedAsync(Exception exception)
//        {
//            UserOnline = false;
//            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
//            await base.OnDisconnectedAsync(exception);
//        }
//    }
//}
