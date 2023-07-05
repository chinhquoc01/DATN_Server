using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class WorkProposalDTO : WorkDTO
    {
        public string ProposalContent { get; set; }
        public double ExpectedPrice { get; set; }

        public ProposalStatus ProposalStatus { get; set; }
        public Guid ProposalId { get; set; }
    }
}
