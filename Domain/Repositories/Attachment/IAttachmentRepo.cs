using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IAttachmentRepo : IBaseRepository<Attachment>
    {
        Task<List<string>> GetByRef(Guid refId, RefType refType);
    }
}
