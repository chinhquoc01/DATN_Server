using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Review : BaseEntity
    {
        public Guid ReviewerId { get; set; }
        public Guid RevieweeId { get; set; }
        public string Content { get; set; }
        public int Star { get; set; }
        public Guid WorkId { get; set; }
    }
}
