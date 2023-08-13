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
    public class UserRepo : BaseRepository<User>, IUserRepo
    {
        public UserRepo(IConfiguration configuration) : base(configuration)
        {

        }

        public async Task<User> GetUserLogin(LoginParam loginParam)
        {
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = $"SELECT * FROM {typeof(User).Name.ToLower()} WHERE Email = @Email";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add($"@Email", loginParam.Email.Trim());
                var res = await sqlConnection.QueryFirstOrDefaultAsync<User>(sqlCommand, parameters);
                return res;
            }
        }

        public async Task<bool> CheckValidUserSignUp(SignupParam signupParam)
        {
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = $"select * from {typeof(User).Name.ToLower()} where email = @Email or phone = @Phone;";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add($"@Email", signupParam.Email.Trim());
                parameters.Add($"@Phone", signupParam.Phone.Trim());
                var res = await sqlConnection.QueryFirstOrDefaultAsync<User>(sqlCommand, parameters);

                return res == null;
            }
        }

        public async Task<bool> RateUser(Guid userId, double ratePoint, int rateCount)
        {
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                var sql = $"update {typeof(User).Name.ToLower()} set Rating = @ratePoint, RatedCount = @rateCount where Id = @userId;";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@ratePoint", ratePoint);
                parameters.Add("@rateCount", rateCount);
                parameters.Add("@userId", userId.ToString());
                var res = await sqlConnection.ExecuteAsync(sql, parameters);
                return res != 0;
            }
        }

        public async Task<List<User>> GetSuggestFreelancer(string workField, List<string>? listField)
        {
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                var sql = $"select * from {typeof(User).Name.ToLower()} where WorkField = @workField ";

                string skillFilter = "";
                if (listField != null)
                {
                    for (int i = 0; i < listField.Count; i++)
                    {
                        if (i == 0)
                        {
                            skillFilter += $" or (Skills like '%{listField[i].Trim()}%' ";
                        }
                        else
                        {
                            skillFilter += $"OR Skills like '%{listField[i].Trim()}%' ";
                        }
                        if (i == listField.Count - 1)
                        {
                            skillFilter += ")";
                        }
                    }
                }
                sql += skillFilter;

                sql += " order by Rating desc; ";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@workField", workField);
                var res = await sqlConnection.QueryAsync<User>(sql, parameters);
                return res.ToList();
            }
        }
    }
}
