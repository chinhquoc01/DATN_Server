using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class User : BaseEntity
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public UserType UserType { get; set; }

        public string Password { get; set; }

        public string? JobTitle { get; set; }

        public string? Description { get; set; }

        public double HourlyRate { get; set; }

        public string? Skills { get; set; }
        public double Rating { get; set; }
        public int RatedCount { get; set; }

        public string IdentityNumber { get; set; }
        public string BankNumber { get; set; }
        public string BankName { get; set; }

        public string WorkField { get; set; }
    }


}
