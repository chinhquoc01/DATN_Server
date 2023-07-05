using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AWS
{
    public class AmazonS3Service : IAmazonS3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly IConfiguration _config;
        private string _bucketName;
        public AmazonS3Service(IAmazonS3 s3Client, IConfiguration config)
        {
            _s3Client = s3Client;
            _config = config;
            _bucketName = _config.GetSection("AWS")["BucketName"];
        }

        public async Task<bool> UploadFile(IFormFile file, string fileKey, string prefix)
        {
            var bucketExists = await _s3Client.DoesS3BucketExistAsync(_bucketName);
            if (!bucketExists) return false;
            
            var request = new PutObjectRequest()
            {
                BucketName = _bucketName,
                Key = fileKey,
                InputStream = file.OpenReadStream()
            };
            request.Metadata.Add("Content-Type", file.ContentType);
            try
            {
                var res = await _s3Client.PutObjectAsync(request);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<GetObjectResponse> GetFileByKeyAsync(string key)
        {
            var bucketExists = await _s3Client.DoesS3BucketExistAsync(_bucketName);
            if (!bucketExists) return null;
            var s3Object = await _s3Client.GetObjectAsync(_bucketName, key);
            return s3Object;
        }

        public async Task<DeleteObjectResponse> DeleteFileAsync(string key)
        {
            var bucketExists = await _s3Client.DoesS3BucketExistAsync(_bucketName);
            if (!bucketExists) return null;
            var res = await _s3Client.DeleteObjectAsync(_bucketName, key);
            return res;
        }

        private static string RandomString()
        {
            string path = Path.GetRandomFileName();
            path = path.Replace(".", "").Replace("/", ""); // Remove period.
            return path.Substring(0, 8);  // Return 8 character string
        }
    }
}
