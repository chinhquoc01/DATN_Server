using Domain.Entities;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class EmploymentHistoryService : BaseService<EmploymentHistory>, IEmployeementHistoryService
    {
        public EmploymentHistoryService(IBaseRepository<EmploymentHistory> baseRepository) : base(baseRepository)
        {
        }
    }
}
