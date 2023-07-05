using Application.AWS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IAmazonEmailService _amazonEmailService;

        public TestController(IAmazonEmailService amazonEmailService)
        {
            _amazonEmailService = amazonEmailService;
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail(List<string> toAddresses,
            string bodyHtml, string bodyText, string subject, string senderAddress)
        {
            try
            {
                var ccAddresses = new List<string>();
                var bccAddresses = new List<string>();
                var res = await _amazonEmailService.SendEmailAsync(toAddresses, ccAddresses, bccAddresses, bodyHtml, bodyText, subject, senderAddress);
                return Ok(res);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
