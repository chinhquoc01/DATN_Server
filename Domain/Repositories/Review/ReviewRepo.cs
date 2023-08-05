using Dapper;
using Domain.DTOs;
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
    public class ReviewRepo : BaseRepository<Review>, IReviewRepo
    {
        public ReviewRepo(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<List<ReviewDTO>> GetReviewHistory(Guid userId)
        {
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = @"select r.*, u.Name ReviewerName, u2.Name RevieweeName, w.Title WorkTitle from review r 
                                    left join `user` u on r.ReviewerId = u.Id 
                                    left join `user` u2 on r.RevieweeId = u2.Id 
                                    left join `work` w on r.WorkId = w.Id 
                                    where r.RevieweeId = @userId; ";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add($"@userId", userId.ToString());
                var res = await sqlConnection.QueryAsync<ReviewDTO>(sqlCommand, parameters);
                return res.ToList();
            }
        }
    }
}
