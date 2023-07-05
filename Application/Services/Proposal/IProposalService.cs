using Domain.DTOs;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IProposalService : IBaseService<Proposal>
    {
        Task<List<ProposalDTO>> GetProposalByWorkId(Guid workId);

        Task<bool> AcceptProposal(Guid proposalId);

        Task<int> UpdateStatusProposal(Guid proposalId, ProposalStatus proposalStatus);
    }
}
