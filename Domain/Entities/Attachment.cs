using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Attachment : BaseEntity
    {
        public string Key { get; set; }
        public string Link { get; set; }
        public Guid RefId { get; set; }

        public RefType RefType { get; set; }

        public Guid CreatedBy { get; set; }

    }
    

    public enum RefType
    {
        JD,
        Proposal,
        WorkProgress,
        Contract,
        Avatar,
        UserInfo
    }
}
