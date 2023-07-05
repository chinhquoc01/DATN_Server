using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractsController : BaseController<Contract>
    {
        private readonly IContractService _contractService;
        public ContractsController(IContractService contractService) : base(contractService)
        {
            _contractService = contractService;
        }

        [HttpGet("freelancer")]
        public async Task<IActionResult> GetByFreelancerId(Guid freelancerId)
        {
            try
            {
                var res = await _contractService.GetByFreelancerId(freelancerId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("detail")]
        public async Task<IActionResult> GetContractDetail(Guid contractId)
        {
            try
            {
                var res = await _contractService.GetContractDetail(contractId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("approve")]
        public async Task<IActionResult> ApproveContract(Guid contractId, Guid freelancerId)
        {
            try
            {
                var res = await _contractService.ApproveContract(contractId, freelancerId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        
        [HttpPost("reject")]
        public async Task<IActionResult> RejectContract(Guid contractId, Guid freelancerId)
        {
            try
            {
                var res = await _contractService.RejectContract(contractId, freelancerId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
