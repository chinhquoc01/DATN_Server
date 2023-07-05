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
    public class ContractRepo : BaseRepository<Contract>, IContractRepo
    {
        public ContractRepo(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<bool> ApproveContract(Guid contractId, Guid freelancerId, Contract contractDetail)
        {
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = $"update contract c set c.Status = @status where c.Id = @contractId; " +
                    $"update `work` w set w.FreelancerId = @freelancerId, w.Status = @workStatus where w.Id = @workId; " +
                    $"update proposal p set p.Status = @proposalStatus where p.Id = @proposalId;";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@status", (int)ContractStatus.Valid);
                parameters.Add("@contractId", contractId.ToString());
                parameters.Add("@freelancerId", freelancerId.ToString());
                parameters.Add("@workId", contractDetail.WorkId.ToString());
                parameters.Add("@proposalId", contractDetail.ProposalId.ToString());
                parameters.Add("@proposalStatus", (int)ProposalStatus.Accept);
                parameters.Add("@workStatus", (int)WorkStatus.InProgress);
                
                var res = await sqlConnection.ExecuteAsync(sqlCommand, parameters);
                return res != 0;


            }
        }

        public async Task<ContractDTO> GetContractDetail(Guid contractId)
        {
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = $"select w.Title as JobTitle, w.Description as JobDescription," +
                    $"u.Name ClientName, u.Address ClientAddress, u.Avatar ClientAvatar, u.Email ClientEmail, u.Phone ClientPhone," +
                    $"c.* from contract c " +
                    $"inner join `work` w on c.WorkId = w.Id " +
                    $"inner join `user` u on u.Id = c.ClientId " +
                    $"where c.Id = @contractId; ";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@contractId", contractId.ToString());
                var res = await sqlConnection.QueryFirstOrDefaultAsync<ContractDTO>(sqlCommand, parameters);
                if (res != null) return res;
                return null;
            }
        }

        public async Task<bool> RejectContract(Guid contractId, Guid freelancerId)
        {
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = $"update contract c set c.Status = @status where c.Id = @contractId and c.FreelancerId = @freelancerId; ";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@status", (int)ContractStatus.Rejected);
                parameters.Add("@contractId", contractId.ToString());
                parameters.Add("@freelancerId", freelancerId.ToString());

                var res = await sqlConnection.ExecuteAsync(sqlCommand, parameters);
                return res != 0;


            }
        }

        public async Task<int> UpdateContractStatus(Guid contractId, ContractStatus status)
        {
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = $"update contract c set c.Status = @status where c.Id = @contractId;";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@status", (int)status);
                parameters.Add("@contractId", contractId.ToString());

                var res = await sqlConnection.ExecuteAsync(sqlCommand, parameters);
                return res;
            }
        }
    }
}
