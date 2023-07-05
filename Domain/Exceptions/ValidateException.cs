using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class ValidateException : Exception
    {
        private IDictionary ErrorData;

        public string ErrorMsg { get; set; }
        public ValidateException(string errorMsg, IDictionary errorData)
        {
            this.ErrorMsg = errorMsg;
            ErrorData = errorData;
        }

        public ValidateException(IDictionary errorData)
        {
            ErrorData = errorData;
        }

        public ValidateException(string errorMsg)
        {
            this.ErrorMsg = errorMsg;
        }

        public override string Message => this.ErrorMsg;
        public override IDictionary Data => ErrorData;
    }
}
