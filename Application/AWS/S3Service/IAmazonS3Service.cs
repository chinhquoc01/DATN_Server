using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AWS
{
    public interface IAmazonS3Service
    {
        Task<bool> UploadFile(IFormFile file, string fileKey, string prefix);
        Task<GetObjectResponse> GetFileByKeyAsync(string key);
        Task<DeleteObjectResponse> DeleteFileAsync(string key);
    }
}
