using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : BaseController<Review>
    {
        private readonly IReviewService _reviewService;
        public ReviewsController(IReviewService reviewService) : base(reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetReviewHistory(Guid userId)
        {
            try
            {
                var res = await _reviewService.GetReviewHistory(userId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
