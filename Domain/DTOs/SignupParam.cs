using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class SignupParam
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Password { get; set; }

        public UserType UserType { get; set; }

        public string? JobTitle { get; set; }

        public string? Description { get; set; }

        public string? Skills { get; set; }
    }
}
