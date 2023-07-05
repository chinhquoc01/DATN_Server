using Dapper;
using Domain.DTOs;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public class ProposalRepo : BaseRepository<Proposal>, IProposalRepo
    {
        public ProposalRepo(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<bool> AcceptProposal(Proposal proposal)
        {
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = $"update proposal set Status = @proposalStatus, ModifiedDate= @modifiedDate where Id = @proposalId; " +
                    $"update `work` set Status = @workStatus, FreelancerId = @freelancerId, ModifiedDate= @modifiedDate where Id = @workId;";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add($"@proposalStatus", (int)ProposalStatus.Accept);
                parameters.Add($"@proposalId", proposal.Id.ToString().Trim());
                parameters.Add($"@workId", proposal.WorkId.ToString().Trim());
                parameters.Add($"@freelancerId", proposal.FreelancerId.ToString().Trim());
                parameters.Add($"@workStatus", (int)WorkStatus.InProgress);
                parameters.Add($"@modifiedDate", DateTime.Now);
                var res = await sqlConnection.ExecuteAsync(sqlCommand, parameters);
                return res == 2;
            }
        }

        public async Task<List<ProposalDTO>> GetProposalsByWork(Guid workId)
        {
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = $"SELECT p.*, " +
                    $"u.Name as FreelancerName, u.Address as FreelancerAddress, u.Skills, u.JobTitle, u.Avatar, u.Email, u.Phone, u.Rating, u.Address " +
                    $"FROM {typeof(Proposal).Name.ToLower()} p " +
                    $"join user u on p.FreelancerId = u.Id " +
                    $"WHERE WorkId = @WorkId";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add($"@WorkId", workId.ToString().Trim());
                var res = await sqlConnection.QueryAsync<ProposalDTO>(sqlCommand, parameters);
                if (res != null) return res.ToList();
                return null;
            }
        }

        public async Task<int> UpdateProposalStatus(Guid proposalId, ProposalStatus proposalStatus)
        {
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = $"update proposal set Status = @status where Id = @id;";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@status", (int)proposalStatus);
                parameters.Add("@id", proposalId.ToString());
                var res = await sqlConnection.ExecuteAsync(sqlCommand, parameters);
                return res;
            }
        }
    }
}
