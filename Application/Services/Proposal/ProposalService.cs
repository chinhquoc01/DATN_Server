using Application.AWS;
using Domain.DTOs;
using Domain.Entities;
using Domain.Enums;
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
    public class ProposalService : BaseService<Proposal>, IProposalService
    {
        private readonly IProposalRepo _repo;
        private readonly IWorkService _workService;
        private readonly IUserService _userService;
        private readonly IAmazonEmailService _emailService;
        private readonly IConfiguration _config;
        public ProposalService(IProposalRepo repo, IAmazonEmailService emailService, IWorkService workService, IUserService userService, IConfiguration config) : base(repo)
        {
            _repo = repo;
            _emailService = emailService;
            _workService = workService;
            _userService = userService;
            _config = config;
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

        public override async Task<int> Insert(Proposal proposalEntity, MySqlConnection sqlConnection = null, IDbTransaction dbTransaction = null)
        {
            var res = await base.Insert(proposalEntity, sqlConnection, dbTransaction);
            if (res > 0)
            {
                var clientUrl = _config.GetSection("ClientUrl").Value;
                var workInfo = await _workService.GetById(proposalEntity.WorkId);
                var clientInfo = await _userService.GetById(workInfo.ClientId);
                // send email
                var receiverAddress = new List<string>() { clientInfo.Email };
                var proposalUrl = $"{clientUrl}/client/work-detail/{proposalEntity.WorkId}";
                var bodyHtml = @$"<div>Bạn có lời đề nghị làm việc mới, chi tiết <a href={proposalUrl}>Tại đây</a></div>";
                var title = "Lời đề nghị làm việc mới";
                _emailService.SendEmailAsync(receiverAddress, bodyHtml, "", title);
            }
            return res;

        }
    }
}
