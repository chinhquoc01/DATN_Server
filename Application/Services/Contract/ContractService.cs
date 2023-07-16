using Application.AWS;
using Domain.DTOs;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Configuration;
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
        private IAmazonEmailService _emailService;
        private readonly IUserService _userService;
        private readonly IConfiguration _config;
        public ContractService(IContractRepo contractRepo, IProposalService proposalService, IAmazonEmailService emailService, IUserService userService, IConfiguration config) : base(contractRepo)
        {
            _proposalService = proposalService;
            _contractRepo = contractRepo;
            _emailService = emailService;
            _userService = userService;
            _config = config;
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
            var contractInfo = await GetById(contractId);
            _proposalService.UpdateStatusProposal(contractInfo.ProposalId, Domain.Enums.ProposalStatus.Reject);
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

                // send email
                var clientUrl = _config.GetSection("ClientUrl").Value;
                var freelancerInfo = await _userService.GetById(contract.FreelancerId);
                var receiverAddress = new List<string>() { freelancerInfo.Email };
                var proposalUrl = $"{clientUrl}/freelancer/contracts?contractId={contract.Id}";
                var bodyHtml = @$"<div>Bạn có nhận được hợp đồng mới, chi tiết <a href={proposalUrl}>Tại đây</a></div>";
                var title = "Hợp đồng làm việc mới";
                _emailService.SendEmailAsync(receiverAddress, bodyHtml, "", title);
            }
            return res;
        }
    }
}
