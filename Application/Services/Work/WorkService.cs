using Application.AWS;
using Domain.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class WorkService : BaseService<Work>, IWorkService
    {
        private readonly IWorkRepo _workRepo;
        private readonly IAmazonEmailService _amazonEmailService;
        private readonly IUserService _userService;
        private readonly IConfiguration _config;
        public WorkService(IWorkRepo workRepo, IAmazonEmailService amazonEmailService, IUserService userService, IConfiguration config) : base(workRepo)
        {
            _workRepo = workRepo;
            _amazonEmailService = amazonEmailService;
            _userService = userService;
            _config = config;
        }

        public async Task<List<WorkDTO>> GetByClientId(Guid clientId, WorkStatus? workStatus = null, WorkType? workType = null)
        {
            var res = await _workRepo.GetByClientId(clientId, workStatus, workType);
            res = res.OrderByDescending(x => x.CreatedDate).ToList();
            return res;
        }

        public async Task<List<Work>> GetClientWorks(Guid clientId, WorkStatus? workStatus = null)
        {
            var res = await GetByFieldValue("ClientId", clientId.ToString());
            if (workStatus == null) return res;
            return res.FindAll(el => el.Status == workStatus);
        }

        public async Task<List<WorkDTO>> GetForFreelancer(Guid freelancerId, WorkFilter workFilter)
        {
            var res = await _workRepo.GetForFreelancer(freelancerId, workFilter.SkillList, workFilter.Range, workFilter.SearchQuery, workFilter.Type, workFilter.WorkField);
            res = res.OrderByDescending(x => x.CreatedDate).ToList();
            return res;
        }

        public async Task<List<Work>> GetFreelancerWorks(Guid freelancerId, WorkStatus? workStatus = null)
        {
            var res = await GetByFieldValue("FreelancerId", freelancerId.ToString());
            if (workStatus == null) return res;
            return res.FindAll(el => el.Status == workStatus);
        }

        public async Task<List<WorkProposalDTO>> GetProposalList(Guid freelancerId)
        {
            var res = await _workRepo.GetProposalList(freelancerId);
            return res;
        }

        public async Task<WorkFreelancer> GetWorkFreelancerDetail(Guid workId, Guid freelancerId)
        {
            var res = await _workRepo.GetWorkFreelancerDetail(workId, freelancerId);
            return res;
        }

        public async Task<List<Work>> GetWorkHistory(Guid userId)
        {
            var user = await _userService.GetById(userId);
            if (user == null)
            {
                return new List<Work>();
            }
            if (user.UserType == UserType.Client)
            {
                var workList = await GetByFieldValue("ClientId", userId);
                workList = workList.OrderByDescending(el => el.CreatedDate).ToList();
                return workList;
            }
            else if (user.UserType == UserType.Freelancer)
            {
                var workList = await GetByFieldValue("FreelancerId", userId);
                workList = workList.OrderByDescending(el => el.CreatedDate).ToList();
                return workList;
            }
            else return new List<Work>();
        }

        public async Task<int> UpdateWorkProgress(Guid workId, int progress)
        {
            var res = await _workRepo.UpdateWorkProgress(workId, progress);

            // send email
            var clientUrl = _config.GetSection("ClientUrl").Value;
            var workInfo = await GetById(workId);
            var clientInfo = await _userService.GetById(workInfo.ClientId);
            var freelancerInfo = await _userService.GetById(workInfo.FreelancerId);
            var receiverAddress = new List<string>() { clientInfo.Email };
            var workUrl = $"{clientUrl}/client/work-detail/{workId}";
            var bodyHtml = @$"<div>Công việc {workInfo.Title} được cập nhật tiến độ thành {progress}% bởi {freelancerInfo.Name}, chi tiết <a href={workUrl}>Tại đây</a></div>";
            var title = $"Công việc {workInfo.Title} được cập nhật tiến độ {progress}%";
            _amazonEmailService.SendEmailAsync(receiverAddress, bodyHtml, "", title);

            return res;
        }

        public async Task<bool> UpdateWorkStatus(Guid workId, WorkStatus workStatus)
        {
            return await _workRepo.UpdateWorkStatus(workId, workStatus);
        }
    }
}
