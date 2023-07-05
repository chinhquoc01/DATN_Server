using Amazon.S3.Model;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IAttachmentService : IBaseService<Attachment>
    {
        Task<bool> UploadAttachment(IFormFile file, string prefix, Guid refId, RefType refType, Guid createdBy);

        Task<GetObjectResponse> GetByKey(string key);
        Task<List<string>> GetByRef(Guid refId, RefType refType);

        Task<bool> DeleteByKey(string fileKey);
    }
}
