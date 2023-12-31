﻿using Domain.DTOs;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IWorkRepo : IBaseRepository<Work>
    {
        Task<List<WorkDTO>> GetByClientId(Guid clientId, WorkStatus? workStatus = null, WorkType? workType = null);
        Task<List<WorkDTO>> GetForFreelancer(Guid freelancerId, List<string> skillList, List<int> range, string searchQuery, WorkType? workType = null, string workField = null);
        Task<List<WorkProposalDTO>> GetProposalList(Guid freelancerId);

        Task<bool> UpdateWorkStatus(Guid workId, WorkStatus workStatus);
        Task<int> UpdateWorkProgress(Guid workId, int progress);

        Task<WorkFreelancer> GetWorkFreelancerDetail(Guid workId, Guid freelancerId);
    }
}
