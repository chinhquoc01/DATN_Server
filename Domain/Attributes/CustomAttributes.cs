using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Attributes
{
    /// <summary>
    /// Attribute đánh dấu prop không được để trống
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class Required : Attribute
    {
        public string? ErrorValidateMsg;
        public Required()
        {

        }
        public Required(string? errorMsg = "")
        {
            ErrorValidateMsg = errorMsg;
        }
    }

    /// <summary>
    /// Attribute đánh dấu Email phải đúng định dạng
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidEmail : Attribute
    {
    }

    /// <summary>
    /// Custom tên của property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyNameDisplay : Attribute
    {
        public string PropertyName = string.Empty;
        public PropertyNameDisplay(string propName)
        {
            PropertyName = propName;
        }
    }

    /// <summary>
    /// Property không được phép trùng
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NotDuplicate : Attribute
    {
    }

    /// <summary>
    /// Độ dài tối đa 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MaxLength : Attribute
    {
        public int maxLength;
        public MaxLength(int maxLength)
        {
            this.maxLength = maxLength;
        }
    }

    /// <summary>
    /// Regex
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MyRegex : Attribute
    {
        public string Pattern;
        public string PatternExample;
        public MyRegex(string pattern, string patternExample)
        {
            Pattern = pattern;
            PatternExample = patternExample;
        }
    }

    /// <summary>
    /// Định dạng ngày tháng
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MyDate : Attribute
    {

    }
}
