using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum WorkStatus
    {
        New,
        InProgress = 2,
        Completed,
        Cancel,
        Abandon
    }
}
