using Domain.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ProposalService : BaseService<Proposal>, IProposalService
    {
        private readonly IProposalRepo _repo;
        public ProposalService(IProposalRepo repo) : base(repo)
        {
            _repo = repo;
        }

        public async Task<bool> AcceptProposal(Guid proposalId)
        {
            var proposal = await _repo.GetById(proposalId);
            var res = await _repo.AcceptProposal(proposal);
            return res;
        }

        public async Task<List<ProposalDTO>> GetProposalByWorkId(Guid workId)
        {
            var res = await _repo.GetProposalsByWork(workId);
            return res;
        }

        public async Task<int> UpdateStatusProposal(Guid proposalId, ProposalStatus proposalStatus)
        {
            var res = await _repo.UpdateProposalStatus(proposalId, proposalStatus);
            return res;
        }
    }
}
