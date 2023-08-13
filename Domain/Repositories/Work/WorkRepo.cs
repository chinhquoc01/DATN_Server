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
    public class WorkRepo : BaseRepository<Work>, IWorkRepo
    {
        public WorkRepo(IConfiguration configuration) : base(configuration)
        {
        }

        public async  Task<List<WorkDTO>> GetByClientId(Guid clientId, WorkStatus? workStatus = null, WorkType? workType = null)
        {
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = $"SELECT w.*, count(p.id) as ProposalCount, count(m.Id) as messageCount  " +
                    $"FROM {typeof(Work).Name.ToLower()} w " +
                    $"left join proposal p on w.Id = p.WorkId " +
                    $"left join message m on w.Id = m.WorkId and m.ReceiverId = @ClientId " +
                    $"WHERE ClientId = @ClientId ";

                DynamicParameters parameters = new DynamicParameters();
                if (workStatus != null)
                {
                    sqlCommand += " and w.Status = @workStatus ";
                    parameters.Add("@workStatus", (int)workStatus);
                }
                if (workType != null)
                {
                    sqlCommand += " and w.Type = @workType ";
                    parameters.Add("@workType", (int)workType);
                }

                sqlCommand += " group by w.Id;";
                

                parameters.Add($"@ClientId", clientId.ToString().Trim());
                var res = await sqlConnection.QueryAsync<WorkDTO>(sqlCommand, parameters);
                return res.ToList();
            }
        }

        public async Task<List<WorkDTO>> GetForFreelancer(Guid freelancerId, List<string> skillList, List<int> range, string searchQuery, WorkType? workType = null, string workField = null)
        {
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                string skillFilter = "";
                for (int i = 0; i < skillList.Count; i++)
                {
                    if (i == 0)
                    {
                        skillFilter += $"AND (FieldTag like '%{skillList[i].Trim()}%' ";
                    }
                    else
                    {
                        skillFilter += $"OR FieldTag like '%{skillList[i].Trim()}%' ";
                    }
                    if (i == skillList.Count - 1)
                    {
                        skillFilter += ")";
                    }
                }
                var sqlCommand = $"SELECT count(p.Id) as ProposalCount, w.* FROM {typeof(Work).Name.ToLower()} w left join proposal p on w.Id = p.WorkId  " +
                    $"WHERE (w.FreelancerId is null or w.FreelancerId <> @FreelancerId) " +
                    $"and (p.FreelancerId is null or p.FreelancerId <> @FreelancerId) " +
                    $"and w.Status = @workStatus ";
                sqlCommand += skillFilter;
                DynamicParameters parameters = new DynamicParameters(); 
                if (workType != null && (int)workType != -1)
                {
                    sqlCommand += " and w.Type = @workType ";
                    parameters.Add("@workType", (int)workType);
                }
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    sqlCommand += " and (w.Title like @searchQuery or w.Description like @searchQuery or w.Location like @searchQuery) ";
                    parameters.Add("@searchQuery", "%" + searchQuery + "%" );
                }
                if (range != null && range.Count == 2)
                {
                    sqlCommand += " and (w.Budget >= @minRange and w.Budget <= @maxRange) ";
                    parameters.Add("@minRange", range[0]);
                    parameters.Add("@maxRange", range[1]);
                }
                if (!string.IsNullOrEmpty(workField))
                {
                    sqlCommand += " and (w.WorkField is null or w.WorkField = '' or w.WorkField = @workField) ";
                    parameters.Add("@workField", workField);
                }

                sqlCommand += " group by w.Id;";
                parameters.Add($"@FreelancerId", freelancerId.ToString().Trim());
                parameters.Add($"@workStatus", (int)WorkStatus.New);
                var res = await sqlConnection.QueryAsync<WorkDTO>(sqlCommand, parameters);
                return res.ToList();
            }
        }

        public async Task<List<WorkProposalDTO>> GetProposalList(Guid freelancerId)
        {
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                var sqlCommand = $"select w.*, " +
                    $"p.Id as ProposalId, p.Status as ProposalStatus, " +
                    $"p.Content as ProposalContent, p.Price as ExpectedPrice " +
                    $"from {typeof(Work).Name.ToLower()} w " +
                    $"inner join proposal p on w.Id = p.WorkId " +
                    $"where p.FreelancerId = @freelancerId;";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add($"@freelancerId", freelancerId.ToString().Trim());
                var res = await sqlConnection.QueryAsync<WorkProposalDTO>(sqlCommand, parameters);
                return res.ToList();
            }
        }

        public async Task<WorkFreelancer> GetWorkFreelancerDetail(Guid workId, Guid freelancerId)
        {
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                var sql = $"select w.*, c.Budget as ExpectedIncome, c.StartDate, c.EndDate from `work` w " +
                    $"inner join `contract` c on w.Id = c.WorkId " +
                    $"where w.Id = @workId and w.FreelancerId = @freelancerId";

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add($"@freelancerId", freelancerId.ToString().Trim());
                parameters.Add($"@workId", workId.ToString().Trim());
                var res = await sqlConnection.QueryFirstOrDefaultAsync<WorkFreelancer>(sql, parameters);
                return res;
            }
        }

        public async Task<int> UpdateWorkProgress(Guid workId, int progress)
        {
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                var sql = $"update `work` w set w.Progress = @progress where w.Id = @workId";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@progress", progress);
                parameters.Add("@workId", workId.ToString());
                var res = await sqlConnection.ExecuteAsync(sql, parameters);
                if (res != 0) return progress;
                return 0;
            }
        }

        public async Task<bool> UpdateWorkStatus(Guid workId, WorkStatus workStatus)
        {
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                var sql = $"update `work` w set w.Status = @status ";
                DynamicParameters parameters = new DynamicParameters();
                if (workStatus == WorkStatus.InProgress)
                {
                    sql += ", w.StartDate = @startDate ";
                    parameters.Add("@startDate", DateTime.Now);
                } else if (workStatus == WorkStatus.Completed || workStatus == WorkStatus.Abandon)
                {
                    sql += ", w.EndDate = @endDate ";
                    parameters.Add("@endDate", DateTime.Now);
                }
                sql += " where w.Id = @workId;";
                parameters.Add("@status", (int)workStatus);
                parameters.Add("@workId", workId.ToString());
                var res = await sqlConnection.ExecuteAsync(sql, parameters);
                return res != 0;
            }
        }
    }
}
