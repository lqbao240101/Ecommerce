//using Ecommerce.Data.Entities;
//using Ecommerce.Data.Static;
//using Ecommerce.Services.Core.IService;
//using Ecommerce.ViewModels.Common;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Cors;
//using Microsoft.AspNetCore.Mvc;
//using System.Security.Claims;

//namespace Ecommerce.Application.Controllers
//{
//    [EnableCors("CorsPolicy")]
//    [Authorize(AuthenticationSchemes = "Bearer")]
//    [Authorize]
//    [ApiController]
//    [Route("[controller]")]
//    public class ChatRoomController : ControllerBase
//    {
//        protected readonly IChatRoomService _chatRoomService;
//        public ChatRoomController(IChatRoomService chatRoomService)
//        {
//            _chatRoomService = chatRoomService;
//        }
//        [HttpGet]
//        public async Task<IActionResult> Get()
//        {
//            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//            string userRole = User.FindFirstValue(ClaimTypes.Role);
//            try
//            {
//                List<ChatRoom> chatRooms = new();
//                if (userRole.Equals(UserRoles.Admin) || userRole.Equals(UserRoles.SuperAdmin))
//                    chatRooms = await _chatRoomService.AdminGetAllAsync(userId);
//                else if (userRole.Equals(UserRoles.User))
//                    chatRooms = await _chatRoomService.AdminGetAllAsync(userId);
//                return Ok(new EntityResponseMessage
//                {
//                    IsSuccess = true,
//                    Data = chatRooms
//                });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new EntityResponseMessage
//                {
//                    IsSuccess = false,
//                    Message = ex.Message
//                });
//            }
//        }
//        [HttpGet("{findId}")]
//        public async Task<IActionResult> Get(string findId)
//        {
//            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//            string userRole = User.FindFirstValue(ClaimTypes.Role);
//            try
//            {
//                var chatRoom = await _chatRoomService.GetOneAsync(userId, findId, userRole);
//                return Ok(new EntityResponseMessage
//                {
//                    IsSuccess = true,
//                    Data = chatRoom
//                });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new EntityResponseMessage
//                {
//                    IsSuccess = false,
//                    Message = ex.Message
//                });
//            }
//        }
//        [HttpPost]
//        public async void Post(string partNerId)
//        {
//            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//            string userRole = User.FindFirstValue(ClaimTypes.Role);
//            var customerId = userRole.Equals(UserRoles.User) ? userId : partNerId;
//            var adminId = userRole.Equals(UserRoles.User) ? partNerId : userId;
//            var chatRoom = new ChatRoom
//            {
//                AdminId = adminId,
//                CustomerId = customerId,
//            };
//            await _chatRoomService.AddChatRoomAsync(chatRoom);
//        }
//    }
//}
