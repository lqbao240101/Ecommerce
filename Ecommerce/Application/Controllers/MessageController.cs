//using Microsoft.AspNetCore.Cors;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Ecommerce.Services.Core.IService;
//using System.Security.Claims;
//using Ecommerce.Data.Entities;

//namespace Ecommerce.Application.Controllers
//{
//    [EnableCors("CorsPolicy")]
//    [Authorize(AuthenticationSchemes = "Bearer")]
//    [Authorize]
//    [ApiController]
//    [Route("[controller]")]
//    public class MessageController : ControllerBase
//    {
//        private readonly IMessageService _messageService;

//        public MessageController(IMessageService messageService)
//        {
//            _messageService = messageService;
//        }
//        [HttpGet("{roomId}")]
//        public async Task<IActionResult> Get(Guid roomId)
//        {
//            if (roomId == Guid.Empty)
//            {
//                return NotFound();
//            }

//            var messagesForRoom = await _messageService.GetMessagesForChatRoomAsync(roomId);

//            return Ok(messagesForRoom);
//        }

//        // POST api/values
//        [HttpPost]
//        public async void Post(Guid chatRoomId, string message)
//        {
//            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//            var m = new Message()
//            {
//                ChatRoomId = chatRoomId,
//                Contents = message,
//                UserId = userId
//            };
//            await _messageService.AddMessageToRoomAsync(m);
//        }
//    }
//}
