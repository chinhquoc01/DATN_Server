using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.SignalRHub
{
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;

        public ChatHub(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task SendMessage(string message, string senderId, string receiverId, string workId)
        {
            var messageObj = new Message();
            messageObj.Id = Guid.NewGuid();
            messageObj.SenderId = Guid.Parse(senderId);
            messageObj.ReceiverId = Guid.Parse(receiverId);
            if (!string.IsNullOrEmpty(workId))
            {
                messageObj.WorkId = Guid.Parse(workId);
            }
            messageObj.Content = message;
            messageObj.Status = Domain.Enums.MessageStatus.New;
            messageObj.CreatedDate = DateTime.Now;
            messageObj.ModifiedDate = DateTime.Now;
            Clients.All.SendAsync(GetPrivateMessageChannel(senderId, receiverId), messageObj);
            Clients.All.SendAsync(GetPrivateMessageChannel(receiverId, senderId), messageObj);
            Clients.All.SendAsync($"Noti-message-{receiverId}", senderId);
            _messageService.Insert(messageObj);
        }

        private string GetPrivateMessageChannel(string senderId, string receiverId)
        {
            return "Message-" + senderId + "/" + receiverId;
        }
    }
}
