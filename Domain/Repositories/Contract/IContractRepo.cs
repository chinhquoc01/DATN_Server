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
    public interface IContractRepo : IBaseRepository<Contract>
    {
        Task<ContractDTO> GetContractDetail(Guid contractId);

        Task<bool> ApproveContract(Guid contractId, Guid freelancerId, Contract contractDetail);
        Task<bool> RejectContract(Guid contractId, Guid freelancerId);

        Task<int> UpdateContractStatus(Guid contractId, ContractStatus status);

    }
}
