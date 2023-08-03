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
        public ReviewService(IBaseRepository<Review> baseRepository, IUserService userService) : base(baseRepository)
        {
            _userService = userService;
        }

        public override async Task<int> Insert(Review entity, MySqlConnection sqlConnection = null, IDbTransaction dbTransaction = null)
        {
            var res = await base.Insert(entity, sqlConnection, dbTransaction);
            _userService.RateUser(entity.RevieweeId, entity.Star);
            return res;
        }
    }
}
