using AutoMapper;
using Domain.DTOs;
using Domain.Entities;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class MessageService : BaseService<Message>, IMessageService
    {
        private readonly IMessageRepo _repo;
        private readonly IMapper _mapper;
        public MessageService(IMessageRepo repo, IMapper mapper) : base(repo)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<Message>> GetChatHistory(Guid senderId, Guid receiverId)
        {
            var res = await _repo.GetChatHistory(senderId, receiverId);
            return res;
        }

        public async Task<List<UserDTO>> GetUserMessage(Guid userId)
        {
            var listUser = await _repo.GetUserMessage(userId);
            var res = _mapper.Map<List<User>, List<UserDTO>>(listUser);
            return res;
        }
    }
}
