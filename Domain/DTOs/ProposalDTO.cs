using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class ProposalDTO : Proposal
    {
        public string FreelancerName { get; set; }
        public string FreelancerAddress { get; set; }
        public string? Address { get; set; }
        public string? Skills { get; set; }
        public string? JobTitle { get; set; }

        public string? Avatar { get; set; }
        public string Email { get; set; }

        public string Phone { get; set; }
        public double Rating { get; set; }

    }
}
