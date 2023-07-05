using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Resources
{
    public class ResourceVN
    {
        public const string Required_Error = "Thông tin {0} không được phép để trống.";
        public const string Email_Invalid = "Email không đúng định dạng.";
        public const string Max_Length_Error = "{0} không được phép dài quá {1} ký tự.";
        public const string Future_Date_Error = "{0} không thể lớn hơn hiện tại.";
        public const string Regex_Error = "{0} phải có dạng {1}.";
        public const string Default_Validate_Error = "Dữ liệu đầu vào không hợp lệ";
        public const string Error_Exception = "Có lỗi xảy ra";
    }
}
