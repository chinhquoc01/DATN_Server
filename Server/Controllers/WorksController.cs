using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorksController : BaseController<Work>
    {
        private readonly IWorkService _workService;
        public WorksController(IWorkService workService) : base(workService)
        {
            _workService = workService;
        }

        [HttpGet("get-by-client-id")]
        public async Task<IActionResult> GetByClientId(Guid clientId, WorkStatus? workStatus = null, WorkType? workType = null)
        {
            try
            {
                var res = await _workService.GetByClientId(clientId, workStatus, workType);
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

        [HttpPost("suggest-for-freelancer")]
        public async Task<IActionResult> GetForFreelancer(Guid freelancerId, List<string> skillList, double expectIncome, string? searchQuery, WorkType? workType = null)
        {
            try
            {
                if (searchQuery == null) searchQuery = "";
                var res = await _workService.GetForFreelancer(freelancerId, skillList, expectIncome, searchQuery, workType);
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

        [HttpGet("proposal-list")]
        public async Task<IActionResult> GetProposalList(Guid freelancerId)
        {
            try
            {
                var res = await _workService.GetProposalList(freelancerId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateStatus(Guid workId, WorkStatus workStatus)
        {
            try
            {
                var res = await _workService.UpdateWorkStatus(workId, workStatus);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("update-progress")]
        public async Task<IActionResult> UpdateProgress(Guid workId, int progress)
        {
            try
            {
                var res = await _workService.UpdateWorkProgress(workId, progress);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("freelancer")]
        public async Task<IActionResult> GetByFreelancerId(Guid freelancerId, WorkStatus? workStatus = null)
        {
            try
            {
                var res = await _workService.GetFreelancerWorks(freelancerId, workStatus);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("detail-freelancer")]
        public async Task<IActionResult> GetWorkFreelancerDetail(Guid workId, Guid freelancerId)
        {
            try
            {
                var res = await _workService.GetWorkFreelancerDetail(workId, freelancerId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("client")]
        public async Task<IActionResult> GetByClientId(Guid clientId, WorkStatus? workStatus = null)
        {
            try
            {
                var res = await _workService.GetClientWorks(clientId, workStatus);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
