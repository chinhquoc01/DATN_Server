using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Proposal : BaseEntity
    {
        public Guid FreelancerId { get; set; }
        public Guid WorkId { get; set; }
        public string Content { get; set; }
        public ProposalStatus Status { get; set; }
        public double Price { get; set; }

    }

}
