using Application.Services;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    public class MessagesController : BaseController<Message>
    {
        private readonly IMessageService _messageService;
        public MessagesController(IMessageService messageService) : base(messageService)
        {
            _messageService = messageService;
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUserMessage(Guid userId)
        {
            try
            {
                var res = await _messageService.GetUserMessage(userId);
                return Ok(res);
            }
            catch (ValidateException ex)
            {
                return HandleValidateException(ex);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetChatHistory(Guid senderId, Guid receiverId)
        {
            try
            {
                var res = await _messageService.GetChatHistory(senderId, receiverId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
