using Domain.DTOs;
using Domain.Entities;
using Domain.Repositories;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ContractService : BaseService<Contract>, IContractService
    {
        private IProposalService _proposalService;
        private IContractRepo _contractRepo;
        public ContractService(IContractRepo contractRepo, IProposalService proposalService) : base(contractRepo)
        {
            _proposalService = proposalService;
            _contractRepo = contractRepo;
        }

        public async Task<List<Contract>> GetByFreelancerId(Guid freelancerId)
        {
            var contractList = await base.GetByFieldValue("FreelancerId", freelancerId);
            foreach (var contract in contractList)
            {
                if (contract.Status == Domain.Enums.ContractStatus.New)
                {
                    var tenDayAgo = DateTime.Now.AddDays(-10);
                    if (contract.CreatedDate < tenDayAgo)
                    {
                        contract.Status = Domain.Enums.ContractStatus.Expired;
                        _contractRepo.UpdateContractStatus(contract.Id, Domain.Enums.ContractStatus.Expired);
                    }
                }
            }
            return contractList;
        }

        public async Task<bool> ApproveContract(Guid contractId, Guid freelancerId)
        {
            var contractDetail = await _contractRepo.GetById(contractId);
            var res = await _contractRepo.ApproveContract(contractId, freelancerId, contractDetail);
            return res;
        }
        public async Task<bool> RejectContract(Guid contractId, Guid freelancerId)
        {
            var res = await _contractRepo.RejectContract(contractId, freelancerId);
            return res;
        }

        public async Task<ContractDTO> GetContractDetail(Guid contractId)
        {
            var res = await _contractRepo.GetContractDetail(contractId);
            return res;
        }

        public override async Task<int> Insert(Contract contract, MySqlConnection sqlConnection = null, IDbTransaction dbTransaction = null)
        {
            var res = await base.Insert(contract);
            if (res > 0)
            {
                // cập nhật trạng thái ứng tuyển
                var updateProposal = await _proposalService.UpdateStatusProposal(contract.ProposalId, Domain.Enums.ProposalStatus.Negotiating);
            }
            return res;
        }
    }
}
