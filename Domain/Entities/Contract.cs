using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Contract : BaseEntity
    {
        public string? ContractName { get; set; }
        public Guid WorkId { get; set; }
        public Guid ClientId { get; set; }
        public Guid FreelancerId { get; set; }
        public Guid ProposalId { get; set; }
        public double Budget { get; set; }
        public BudgetType BudgetType { get; set; }
        public ContractStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
