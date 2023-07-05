using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public class BaseRepository<Entity> : IBaseRepository<Entity> where Entity : class
    {
        private readonly IConfiguration _config;
        protected readonly string? _connectionString;
        public BaseRepository(IConfiguration configuration)
        {
            _config = configuration;
            _connectionString = _config.GetConnectionString("DefaultConnection");
        }
        private string _tableName = typeof(Entity).Name.ToLower();
        public async Task<int> Insert(Entity entity, MySqlConnection sqlConnection = null, IDbTransaction dbTransaction = null)
        {
            var haveConnection = sqlConnection != null;
            if (!haveConnection)
            {
                sqlConnection = new MySqlConnection(_connectionString);
                sqlConnection.Open();
            }
            //using (var sqlConnection = new MySqlConnection(_connectionString))
            //{
            if (entity.GetType().GetProperty("Id").GetValue(entity) == null)
            {
                entity.GetType().GetProperty($"Id").SetValue(entity, Guid.NewGuid());   
            }
            entity.GetType().GetProperty("CreatedDate").SetValue(entity, DateTime.Now);
            entity.GetType().GetProperty("ModifiedDate").SetValue(entity, DateTime.Now);

            var sqlCommandText = $"Proc_Insert_{_tableName}";

            

            var sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandText = sqlCommandText;
            sqlCommand.CommandType = CommandType.StoredProcedure;
            MySqlCommandBuilder.DeriveParameters(sqlCommand);

            var dynamicParam = new DynamicParameters();
            foreach (MySqlParameter parameter in sqlCommand.Parameters)
            {
                var paramName = parameter.ParameterName;
                var propName = paramName.Replace("@m_", "");
                var entityProperty = entity.GetType().GetProperty(propName);
                if (entityProperty != null)
                {
                    var propValue = entityProperty.GetValue(entity);
                    dynamicParam.Add(paramName, propValue);
                }
                else
                {
                    dynamicParam.Add(paramName, null);
                }
            }
            if (dbTransaction == null)
            {
                var res = await sqlConnection.ExecuteAsync(sql: sqlCommandText, param: dynamicParam, commandType: CommandType.StoredProcedure);
                if (!haveConnection) sqlConnection.Close();
                return res;
            }
            else
            {
                var res = await sqlConnection.ExecuteAsync(sql: sqlCommandText, param: dynamicParam, commandType: CommandType.StoredProcedure, transaction: dbTransaction);
                if (!haveConnection) sqlConnection.Close();
                return res;
            }
            //}
        }
        public async Task<int> Delete(Guid entityId)
        {
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = $"DELETE FROM {_tableName} WHERE Id = @Id";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add($"@Id", entityId);
                var res = await sqlConnection.ExecuteAsync(sqlCommand, parameters);
                return res;
            }
        }

        public async Task<int> DeleteByField(string fieldName, object value)
        {
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = $"DELETE FROM {_tableName} WHERE `{fieldName}` = @value";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add($"@value", value);
                var res = await sqlConnection.ExecuteAsync(sqlCommand, parameters);
                return res;
            }
        }

        public async Task<IEnumerable<Entity>> Get()
        {
            // Khởi tạo kết nối
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {

                var sqlCommand = $"SELECT * FROM {_tableName} ORDER BY CreatedDate DESC";

                var entities = await sqlConnection.QueryAsync<Entity>(sqlCommand);
                return entities;
            }
        }

        public async Task<Entity> GetById(Guid id)
        {
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = $"SELECT * FROM {_tableName} WHERE Id = @Id";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add($"@Id", id);
                var res = await sqlConnection.QueryFirstOrDefaultAsync<Entity>(sqlCommand, parameters);
                return res;
            }
        }
        
        public async Task<List<Entity>> GetByFieldValue(string fieldName, object value)
        {
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = $"SELECT * FROM {_tableName} WHERE `{fieldName}` = @value";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add($"@value", value);
                var res = await sqlConnection.QueryAsync<Entity>(sqlCommand, parameters);
                return res.ToList();
            }
        }
        public async Task<int> Update(Guid entityId, Entity entity)
        {
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                // Đặt entityId bằng entityId cũ
                entity.GetType().GetProperty($"Id").SetValue(entity, entityId);
                // Cập nhật trường ModifiedDate
                entity.GetType().GetProperty("ModifiedDate").SetValue(entity, DateTime.Now);

                var sqlCommandText = $"Proc_Update_{_tableName}";

                sqlConnection.Open();

                var sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = sqlCommandText;
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                MySqlCommandBuilder.DeriveParameters(sqlCommand);

                var dynamicParam = new DynamicParameters();
                foreach (MySqlParameter parameter in sqlCommand.Parameters)
                {
                    var paramName = parameter.ParameterName;
                    var propName = paramName.Replace("@m_", "");
                    var entityProperty = entity.GetType().GetProperty(propName);
                    if (entityProperty != null)
                    {
                        var propValue = entity.GetType().GetProperty(propName).GetValue(entity);
                        dynamicParam.Add(paramName, propValue);
                    }
                    else
                    {
                        dynamicParam.Add(paramName, null);
                    }
                }

                var res = await sqlConnection.ExecuteAsync(sql: sqlCommandText, param: dynamicParam, commandType: System.Data.CommandType.StoredProcedure);
                return res;

            }
        }

        public async Task<int> GetTotalRecord()
        {
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = $"SELECT COUNT(Id) FROM {_tableName}";
                var totalRecord = await sqlConnection.QueryFirstOrDefaultAsync<int>(sqlCommand);
                return totalRecord;
            }
        }

    }   
}
