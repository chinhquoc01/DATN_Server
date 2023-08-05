using Domain.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IReviewRepo : IBaseRepository<Review>
    {
        Task<List<ReviewDTO>> GetReviewHistory(Guid userId);
    }
}
