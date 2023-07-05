using Application.Services;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProposalsController : BaseController<Proposal>
    {
        private readonly IProposalService _proposalService;
        public ProposalsController(IProposalService service) : base(service)
        {
            _proposalService = service;
        }

        [HttpGet("work/{workId}")]
        public async Task<IActionResult> GetByWorkId(Guid workId)
        {
            try
            {
                var res = await _proposalService.GetProposalByWorkId(workId);
                return Ok(res);
            }
            catch (ValidateException ex)
            {
                return HandleValidateException(ex);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("accept/{proposalId}")]
        public async Task<IActionResult> AcceptProposal(Guid proposalId)
        {
            try
            {
                var res = await _proposalService.AcceptProposal(proposalId);
                return Ok(res);
            }
            catch (ValidateException ex)
            {
                return HandleValidateException(ex);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
