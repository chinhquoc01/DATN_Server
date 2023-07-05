using Domain.Attributes;
using Domain.Exceptions;
using Domain.Repositories;
using Domain.Resources;
using MySqlConnector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Services
{
    public class BaseService<Entity> : IBaseService<Entity> where Entity : class
    {
        IBaseRepository<Entity> _baseRepository;
        protected IDictionary errorData;
        private ResourceVN resourceVn = new ResourceVN();
        public BaseService(IBaseRepository<Entity> baseRepository)
        {
            _baseRepository = baseRepository;
            errorData = new Dictionary<string, List<string>>();
        }

        /// <summary>
        /// Validate dữ liệu trước khi thêm mới hoặc sửa
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="oldEntity">null khi ở chế độ thêm mới, khác null khi ở chế độ sửa</param>
        /// <returns>
        /// true - dữ liệu hợp lệ
        /// fasle - dữ liệu không hợp lệ
        /// </returns>
        /// Created by: QuocPC (15/03/2022)
        private async Task<bool> ValidateObject(Entity entity)
        {
            var isValid = true;

            // 1. Quét toàn bộ các prop của đối tượng cần validate
            var properties = entity.GetType().GetProperties();

            foreach (var property in properties)
            {
                // Lấy tên của prop
                var propName = property.Name;
                // Lấy giá trị của prop
                var propValue = property.GetValue(entity);
                // Lấy ra tên hiển thị của Property
                var propertyNameAttribute = Attribute.GetCustomAttribute(property, typeof(PropertyNameDisplay));

                if (propertyNameAttribute != null)
                {
                    propName = (propertyNameAttribute as PropertyNameDisplay).PropertyName;
                }

                // 1.1 Bắt buộc nhập
                var isRequired = Attribute.IsDefined(property, typeof(Required));
                if (isRequired == true && (propValue == null || propValue.ToString() == string.Empty))
                {
                    isValid = false;
                    var requiredAttributeObject = Attribute.GetCustomAttribute(property, typeof(Required));
                    var errorMsg = (requiredAttributeObject as Required).ErrorValidateMsg;
                    if (errorMsg == null)
                    {
                        errorMsg = String.Format(Domain.Resources.ResourceVN.Required_Error, propName);
                    }

                    AddError(errorData, property.Name, errorMsg);
                }

                // 1.2 Email phải đúng định dạng
                var emailValid = Attribute.IsDefined(property, typeof(ValidEmail));
                if (emailValid == true && (propValue != null && propValue.ToString() != string.Empty))
                {
                    if (!IsValidEmail(propValue.ToString()))
                    {
                        AddError(errorData, property.Name, Domain.Resources.ResourceVN.Email_Invalid);
                    }
                }

                // 1.3 Độ dài tối đa được nhập
                var maxLengthAttribute = Attribute.GetCustomAttribute(property, typeof(MaxLength));
                if (maxLengthAttribute != null && (propValue != null && propValue.ToString() != String.Empty))
                {
                    var maxLength = (maxLengthAttribute as MaxLength).maxLength;

                    if (propValue.ToString().Length > maxLength)
                    {
                        AddError(errorData, property.Name, String.Format(Domain.Resources.ResourceVN.Max_Length_Error, propName, maxLength));
                    }


                }
                // 1.4 Ngày tháng không được lớn hơn hiện tại
                var dateAttribute = Attribute.GetCustomAttribute(property, typeof(MyDate));
                if (dateAttribute != null && (propValue != null && propValue.ToString() != String.Empty))
                {

                    DateTime date = DateTime.Parse(propValue.ToString());
                    if (date > DateTime.Now)
                    {
                        AddError(errorData, property.Name, String.Format(Domain.Resources.ResourceVN.Future_Date_Error, propName));
                    }
                }

              

                // 1.6 Kiểm tra regex
                var regexAttribute = Attribute.GetCustomAttribute(property, typeof(MyRegex));
                if (regexAttribute != null)
                {
                    var pattern = (regexAttribute as MyRegex).Pattern;
                    var patternExample = (regexAttribute as MyRegex).PatternExample;
                    Regex rgx = new Regex(pattern);

                    if (!rgx.IsMatch(propValue.ToString()))
                    {
                        AddError(errorData, property.Name, String.Format(Domain.Resources.ResourceVN.Regex_Error, propName, patternExample));
                    }

                }
            }

            // 2. Xử lý validate riêng
            var isValidCustom = await ValidateCustom(entity);

            if (errorData.Count > 0)
            {
                throw new ValidateException(Domain.Resources.ResourceVN.Default_Validate_Error, errorData);
            }
            return isValid && isValidCustom;
        }

        /// <summary>
        /// Thực hiện validate dữ liệu đặc thù 
        /// </summary>
        /// <param name="entity">Đối tượng cần kiểm tra thông tin</param>
        /// <returns>true - hợp lệ, false - không hợp lệ</returns>
        /// Created by: QuocPC (16/03/2022)
        protected virtual async Task<bool> ValidateCustom(Entity entity)
        {
            return true;
        }

        public virtual async Task<int> Insert(Entity entity, MySqlConnection sqlConnection = null, IDbTransaction dbTransaction = null)
        {
            var isValid = await ValidateObject(entity);
            // validate dữ liệu
            if (isValid == true)
            {
                // Thực hiện thêm mới dữ liệu
                var res = await _baseRepository.Insert(entity, sqlConnection, dbTransaction);
                // Trả về kết quả
                return res;

            }
            else
            {
                throw new Exception(Domain.Resources.ResourceVN.Default_Validate_Error);
            }
        }

        public async Task<int> Update(Guid entityId, Entity entity)
        {
            var isValid = await ValidateObject(entity);
            // validate dữ liệu
            if (isValid == true)
            {
                // Thực hiện thêm mới dữ liệu
                var res = await _baseRepository.Update(entityId, entity);
                // Trả về kết quả
                return res;

            }
            else
            {
                throw new Exception(Domain.Resources.ResourceVN.Default_Validate_Error);
            }
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ của Email
        /// </summary>
        /// <param name="emailaddress"></param>
        /// <returns>
        /// true - email hợp lệ,
        /// false - email không hợp lệ
        /// </returns>
        private bool IsValidEmail(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        /// <summary>
        /// Thêm lỗi vào ErrorData
        /// </summary>
        /// <param name="errorData"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        protected void AddError(IDictionary errorData, string key, string value)
        {
            if (errorData[key] == null)
            {
                List<string> errors = new List<string>();
                errors.Add(value);
                errorData.Add(key, errors);

            }
            else
            {
                List<string> errors = (List<string>)errorData[key];
                errors.Add(value);
            }
        }

        public async Task<IEnumerable<Entity>> Get()
        {
            IEnumerable<Entity> entities = await _baseRepository.Get();
            return entities;
        }

        public async Task<Entity> GetById(Guid entityId)
        {
            Entity entity = await _baseRepository.GetById(entityId);
            return entity;
        }
        public async Task<List<Entity>> GetByFieldValue(string fieldName, object value)
        {
            var entity = await _baseRepository.GetByFieldValue(fieldName, value);
            return entity;
        }

        

        public async Task<int> Delete(Guid entityId)
        {
            int res = await _baseRepository.Delete(entityId);
            return res;
        }
    }
}
