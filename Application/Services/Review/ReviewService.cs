using AutoMapper;
using Domain.DTOs;
using Domain.Entities;
using Domain.Repositories;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ReviewService : BaseService<Review>, IReviewService
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IReviewRepo _reviewRepo;
        public ReviewService(IReviewRepo reviewRepo, IUserService userService, IMapper mapper) : base(reviewRepo)
        {
            _userService = userService;
            _mapper = mapper;
            _reviewRepo = reviewRepo;
        }

        public async Task<List<ReviewDTO>> GetReviewHistory(Guid userId)
        {
            var res = await _reviewRepo.GetReviewHistory(userId);
            return res;
        }

        public override async Task<int> Insert(Review entity, MySqlConnection sqlConnection = null, IDbTransaction dbTransaction = null)
        {
            var res = await base.Insert(entity, sqlConnection, dbTransaction);
            _userService.RateUser(entity.RevieweeId, entity.Star);
            return res;
        }
    }
}
