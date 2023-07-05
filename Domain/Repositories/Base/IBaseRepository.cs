using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IBaseRepository<Entity> where Entity : class
    {
        /// <summary>
        /// Lấy tất cả dữ liệu
        /// </summary>
        /// <returns>
        /// Trả về tất cả bản ghi
        /// </returns>
        /// Created by: QuocPC
        public Task<IEnumerable<Entity>> Get();

        /// <summary>
        /// Xoá bản ghi theo id
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns>
        /// Số bản ghi bị ảnh hưởng
        /// </returns>
        /// Created by: QuocPC
        public Task<int> Delete(Guid entityId);
        public Task<int> DeleteByField(string fieldName, object value);

        /// <summary>
        /// Lấy thông tin bằng id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// Trả về entity
        /// </returns>
        /// Created by: QuocPC
        public Task<Entity> GetById(Guid id);

        Task<List<Entity>> GetByFieldValue(string fieldName, object value);

        /// <summary>
        /// Thêm thông tin
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>
        /// Số bản ghi ảnh hưởng
        /// </returns>
        /// Created by: QuocPC
        public Task<int> Insert(Entity entity, MySqlConnection dbConnection = null, IDbTransaction dbTransaction = null);

        /// <summary>
        /// Cập nhật thông tin
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="entity"></param>
        /// <returns>
        /// Số bản ghi ảnh hưởng
        /// </returns>
        /// Created by: QuocPC
        public Task<int> Update(Guid entityId, Entity entity);

        /// <summary>
        /// Lấy tổng số bản ghi 
        /// </summary>
        /// <returns>
        /// int - tổng số bản ghi của đối tượng trong database
        /// </returns>
        /// Created by: QuocPC 
        public Task<int> GetTotalRecord();
    }
}
