using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class WorkDTO : Work
    {
        public List<Attachment>? Attachments { get; set; }
        public int ProposalCount { get; set; }
        public int MessageCount { get; set; }
    }
}
