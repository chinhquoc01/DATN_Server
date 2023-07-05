using Domain.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IContractService : IBaseService<Contract>
    {
        Task<ContractDTO> GetContractDetail(Guid contractId);

        Task<bool> ApproveContract(Guid contractId, Guid freelancerId);
        Task<bool> RejectContract(Guid contractId, Guid freelancerId);
        Task<List<Contract>> GetByFreelancerId(Guid freelancerId);
    }
}
