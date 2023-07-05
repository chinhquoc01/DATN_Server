using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class ErrorResponse
    {
        /// <summary>
        /// Message cho dev
        /// </summary>
        public string DevMsg { get; set; }

        /// <summary>
        /// Message cho User
        /// </summary>
        public string UserMsg { get; set; }

        /// <summary>
        /// Thông tin lỗi
        /// </summary>
        public object Data { get; set; }
    }
    
}
