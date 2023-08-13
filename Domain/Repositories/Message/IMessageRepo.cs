using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IMessageRepo : IBaseRepository<Message>
    {
        Task<List<User>> GetUserMessage(Guid userId);

        Task<List<Message>> GetChatHistory(Guid senderId, Guid receiverId, int limit, int offset);
    }
}
