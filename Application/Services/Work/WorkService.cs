using Domain.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
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
        public WorkService(IWorkRepo workRepo) : base(workRepo)
        {
            _workRepo = workRepo;
        }

        public async Task<List<WorkDTO>> GetByClientId(Guid clientId, WorkStatus? workStatus = null)
        {
            var res = await _workRepo.GetByClientId(clientId, workStatus);
            res = res.OrderByDescending(x => x.CreatedDate).ToList();
            return res;
        }

        public async Task<List<WorkDTO>> GetForFreelancer(Guid freelancerId, List<string> skillList, double expectIncome)
        {
            var res = await _workRepo.GetForFreelancer(freelancerId, skillList, expectIncome);
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

        public async Task<int> UpdateWorkProgress(Guid workId, int progress)
        {
            return await _workRepo.UpdateWorkProgress(workId, progress);
        }

        public async Task<bool> UpdateWorkStatus(Guid workId, WorkStatus workStatus)
        {
            return await _workRepo.UpdateWorkStatus(workId, workStatus);
        }
    }
}
