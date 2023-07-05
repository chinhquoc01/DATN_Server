using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class WorkFreelancer : Work
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double ExpectedIncome { get; set; }
    }
}
