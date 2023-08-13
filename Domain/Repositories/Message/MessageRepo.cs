using Dapper;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public class MessageRepo : BaseRepository<Message>, IMessageRepo
    {
        public MessageRepo(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<List<Message>> GetChatHistory(Guid senderId, Guid receiverId, int limit, int offset)
        {
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = $"select * from message " +
                    $"where (SenderId = @senderId and ReceiverId = @receiverId) " +
                    $"or (SenderId = @receiverId and ReceiverId = @senderId) " +
                    $"order by CreatedDate desc limit @limit offset @offset;";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add($"@senderId", senderId.ToString().Trim());
                parameters.Add($"@receiverId", receiverId.ToString().Trim());
                parameters.Add($"@limit", limit);
                parameters.Add($"@offset", offset);
                var res = await sqlConnection.QueryAsync<Message>(sqlCommand, parameters);
                if (res != null) return res.ToList();
                return null;
            }
        }

        public async Task<List<User>> GetUserMessage(Guid userId)
        {
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = $"select * from `user` u where Id in " +
                    $"(select distinct if (SenderId = @userId, ReceiverId, SenderId) as userid " +
                    $"from message m " +
                    $"where SenderId = @userId " +
                    $"or ReceiverId = @userId);";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add($"@userId", userId.ToString().Trim());
                var res = await sqlConnection.QueryAsync<User>(sqlCommand, parameters);
                if (res != null) return res.ToList();
                return null;
            }
        }
    }
}
