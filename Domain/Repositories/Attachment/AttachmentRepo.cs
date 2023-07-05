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
    public class AttachmentRepo : BaseRepository<Attachment>, IAttachmentRepo
    {
        public AttachmentRepo(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<List<string>> GetByRef(Guid refId, RefType refType)
        {
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                var sql = $"select `key` from `attachment` where RefId = @refId and RefType = @refType;";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@refId", refId.ToString());
                parameters.Add("@refType", (int)refType);
                var res = await sqlConnection.QueryAsync<string>(sql, parameters);
                return res.ToList();
            }
        }
    }
}
