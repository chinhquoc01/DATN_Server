using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IBaseService<Entity>
    {
        /// <summary>
        /// Service thêm mới bản ghi
        /// </summary>
        /// <param name="entity">Entity cần thêm</param>
        /// <returns>
        /// Số entity thêm được
        /// </returns>
        /// Created by: QuocPC 13/03/2022
        Task<int> Insert(Entity entity, MySqlConnection sqlConnection = null, IDbTransaction dbTransaction = null);

        /// <summary>
        /// Service cập nhật bản ghi
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="entity"></param>
        /// <returns>
        /// Số bản ghi cập nhật được
        /// </returns>
        /// Created by: QuocPC (13/03/2022)
        Task<int> Update(Guid entityId, Entity entity);

        /// <summary>
        /// Service lấy toàn bộ đối tượng trong db
        /// </summary>
        /// <returns>Toàn bộ đối tượng</returns>
        /// Created by: QuocPC (13/03/2022)
        Task<IEnumerable<Entity>> Get();

        /// <summary>
        /// Service lấy 1 đối tượng theo Id
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns>Đối tượng lấy được</returns>
        /// Created by: QuocPC (13/03/2022)
        Task<Entity> GetById(Guid entityId);

        Task<List<Entity>> GetByFieldValue(string fieldName, object value);

        /// <summary>
        /// Service xoá 1 bản ghi theo Id
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns>Số đối tượng bị ảnh hưởng</returns>
        /// Created by: QuocPC (13/03/2022)
        Task<int> Delete(Guid entityId);
    }
}
