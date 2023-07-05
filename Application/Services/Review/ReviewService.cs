using Domain.Entities;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ReviewService : BaseService<Review>, IReviewService
    {
        public ReviewService(IBaseRepository<Review> baseRepository) : base(baseRepository)
        {
        }
    }
}
