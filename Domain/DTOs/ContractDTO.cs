using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class ContractDTO : Contract
    {
        public string? JobTitle { get; set; }
        public string? JobDescription { get; set; }
        public string? ClientName { get; set; }
        public string? ClientAddress { get; set; }
        public string? ClientAvatar { get; set; }
        public string? ClientEmail { get; set; }
        public string? ClientPhone { get; set; }
    }
}
