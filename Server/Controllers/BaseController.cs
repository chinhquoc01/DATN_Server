using Application.Services;
using Domain.DTOs;
using Domain.Enums;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController<Entity> : ControllerBase where Entity : class
    {
        private readonly IBaseService<Entity> _baseService;

        public BaseController(IBaseService<Entity> baseService)
        {
            this._baseService = baseService;
        }

        /// <summary>
        /// Lấy tất cả đối tượng trong db
        /// </summary>
        /// <returns>
        /// 200 - Danh sách đối tượng
        /// 500 - Lỗi bên server
        /// </returns>
        /// Created by: QuocPC (10/03/2022)
        [HttpGet]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var entities = await _baseService.Get();

                return Ok(ToPascalCase(entities));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }


        /// <summary>
        /// Lấy đối tượng bằng Id
        /// </summary>
        /// <param name="entityId">Khoá chính của đối tượng</param>
        /// <returns>
        /// 200 - Trả về thông tin đối tượng
        /// 204 - Không tìm thấy đối tượng
        /// 500 - Lỗi bên server
        /// </returns>
        /// Created by: QuocPC (10/03/2022)
        [HttpGet("{entityId}")]
        public async Task<IActionResult> GetById(Guid entityId)
        {
            try
            {
                var entity = await _baseService.GetById(entityId);
                if (entity != null)
                {
                    return Ok(ToPascalCase(entity));
                }
                else
                    return StatusCode(204);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Thêm mới đối tượng
        /// </summary>
        /// <param name="entity">Đối tượng</param>
        /// <returns>
        /// 201 - Thêm mới thành công
        /// 400 - Dữ liệu đầu vào không hợp lệ
        /// 500 - Internal server error
        /// </returns>
        /// Created by: QuocPC (10/03/2022)
        [HttpPost]
        public async Task<IActionResult> Post(Entity entity)
        {
            try
            {
                var res = await _baseService.Insert(entity);
                if (res > 0)
                {
                    return StatusCode(200, res);
                }
                return Ok();
            }
            catch (ValidateException ex)
            {
                return HandleValidateException(ex);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Xoá đối tượng theo Id
        /// </summary>
        /// <param name="entityId">Khoá chính của đối tượng</param>
        /// <returns>
        /// 200 - Xoá thành công
        /// 204 - Không tìm thấy đối tượng cần xoá
        /// 500 - Lỗi server
        /// </returns>
        /// Created by: QuocPC (16/03/2022)
        [HttpDelete("{entityId}")]
        public async Task<IActionResult> Delete(Guid entityId)
        {
            try
            {
                var res = await _baseService.Delete(entityId);
                if (res > 0)
                {
                    return Ok(res);
                }
                else
                    return StatusCode(204);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }


        /// <summary>
        /// Sửa đối tượng
        /// </summary>
        /// <param name="entityId">Khoá chính của đối tượng</param>
        /// <param name="entity">Đối tượng</param>
        /// <returns>
        /// 200 - Cập nhật thông tin thành công
        /// 204 - Không tìm thấy đối tượng cần sửa
        /// 400 - Dữ liệu sửa không hợp lệ
        /// 500 - Lỗi bên server
        /// </returns>
        /// Created by: QuocPC (10/03/2022)
        [HttpPut("{entityId}")]
        public async Task<IActionResult> Put(Guid entityId, [FromBody] Entity entity)
        {
            try
            {
                var res = await _baseService.Update(entityId, entity);
                if (res > 0)
                {
                    return Ok(res);
                }
                else
                {
                    return StatusCode(500);
                }
                
            }
            catch (ValidateException ex)
            {
                return HandleValidateException(ex);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Xử lý lỗi validate
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <returns>
        /// 400 - BadRequest
        /// Object ErrorService
        /// </returns>
        /// Created by: QuocPC (10/03/2022)
        protected IActionResult HandleValidateException(ValidateException ex)
        {
            // Lấy ra lỗi đầu tiên gặp phải rồi gán cho userMsg
            var e = ex.Data.GetEnumerator();
            e.MoveNext();
            DictionaryEntry userMsgPair = (DictionaryEntry)e.Current;
            var userMsg = (List<string>)userMsgPair.Value;

            var error = new ErrorResponse
            {
                DevMsg = ex.Message,
                Data = ex.Data,
                UserMsg = userMsg.FirstOrDefault(),
            };
            return BadRequest(error);
        }

        /// <summary>
        /// Xử lý exception
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <returns>
        /// 500 - Internal Server Error
        /// </returns>
        /// Created by: QuocPC (10/03/2022)
        protected IActionResult HandleException(Exception ex)
        {
            var error = new ErrorResponse
            {
                DevMsg = ex.Message,
                UserMsg = Domain.Resources.ResourceVN.Error_Exception
            };
            return StatusCode(500, error);
        }

        /// <summary>
        /// Chuyển chuỗi JSON sang dạng PascalCase
        /// </summary>
        /// <param name="o">Object cần chuyển</param>
        /// <returns>JSON string</returns>
        /// Created by: QuocPC (18/03/2022)
        protected object ToPascalCase(object o)
        {
            return o;
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };
            return JsonSerializer.Serialize(o, options);
        }
    }
}
