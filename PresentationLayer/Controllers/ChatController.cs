using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Etities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PresentationLayer.DTOs;

namespace PresentationLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : Controller
    {
        private IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet("getAll")]
        public IActionResult GetAllChats()
        {
            var chats = _chatService.FindAll().ToList();
            return Ok(chats);
        }

        [HttpPost("create")]
        public IActionResult CreateChat(ChatDto chatDto)
        {
            var chat = new Chat() { Name = chatDto.Name, AuthorName = chatDto.AuthorName };

            var result = _chatService.Create(chat);
            if (!result.Success)
                return BadRequest(new { message = result.ErrorMessage });
            else
                return Ok(result.Data);
        }

        [HttpPost("update")]
        public IActionResult UpdateChat(ChatDto chatDto)
        {
            try
            {
                var chat = _chatService.FindByCondition(c => c.Id == chatDto.Id).FirstOrDefault();
                if (chat == null)
                {
                    return BadRequest(new { message = "No chats found." });
                }
                chat.AuthorName = chatDto.AuthorName;
                chat.Name = chatDto.Name;


                var result = _chatService.Update(chat);
                if (!result.Success)
                    return BadRequest(new { message = result.ErrorMessage });
                else
                    return Ok(result.Data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("removeById")]
        public IActionResult RemoveChatById(int id)
        {
            var chat = _chatService.FindByCondition(c => c.Id == id).FirstOrDefault();
            if (chat is not null)
            {
                var result = _chatService.Remove(chat);
                if (!result.Success)
                    return BadRequest(new { message = result.ErrorMessage });
                else
                    return Ok(result.Data);
            }
            return BadRequest(new { message = "The chat does not exist." });
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchChat(string search = "")
        {
            var chats = await _chatService.FindByCondition(c => c.Name.Contains(search)).Select(c => c.Name).ToListAsync();

            if (chats.Count() == 0)
                return BadRequest(new { message = "No chats found." });
            else
                return Ok(chats);
        }
    }
}
