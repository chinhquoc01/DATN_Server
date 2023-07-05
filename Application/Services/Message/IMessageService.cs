using Domain.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IMessageService : IBaseService<Message>
    {
        Task<List<UserDTO>> GetUserMessage(Guid userId);

        Task<List<Message>> GetChatHistory(Guid senderId, Guid receiverId);
    }
}
