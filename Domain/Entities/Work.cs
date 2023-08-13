using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Work : BaseEntity
    {
        public WorkType Type { get; set; }
        public WorkStatus Status { get; set; }
        public int Progress { get; set; }
        public Guid ClientId { get; set; }
        public Guid FreelancerId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public double Budget { get; set; }
        public string? FieldTag { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string WorkField { get; set; }
    }
}
