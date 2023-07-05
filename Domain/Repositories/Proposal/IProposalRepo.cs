using Domain.DTOs;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IProposalRepo : IBaseRepository<Proposal>
    {
        Task<List<ProposalDTO>> GetProposalsByWork(Guid workId);

        Task<bool> AcceptProposal(Proposal proposal);

        Task<int> UpdateProposalStatus(Guid proposalId, ProposalStatus proposalStatus);
    }
}
