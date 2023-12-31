﻿using Domain.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IReviewService : IBaseService<Review>
    {
        Task<List<ReviewDTO>> GetReviewHistory(Guid userId);
    }
}
