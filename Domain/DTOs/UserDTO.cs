using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class UserDTO
    {

        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Avatar { get; set; }
        public Guid Id { get; set; }

        public UserType UserType { get; set; }

        public string Skills { get; set; }
        public double HourlyRate { get; set; }
        public double Rating { get; set; }


    }
}
