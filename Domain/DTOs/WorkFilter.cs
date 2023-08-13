using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class WorkFilter
    {
        public List<int>? Range { get; set; }
        public string? SearchQuery { get; set; }
        public WorkType? Type { get; set; }

        public List<string>? SkillList { get; set; }

        public string WorkField { get; set; }
    }
}
