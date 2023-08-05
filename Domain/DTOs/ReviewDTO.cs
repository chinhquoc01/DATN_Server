using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class ReviewDTO : Review
    {
        public string ReviewerName { get; set; }
        public string RevieweeName { get; set; }
        public string WorkTitle { get; set; }
    }
}
