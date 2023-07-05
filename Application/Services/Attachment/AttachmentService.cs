using Amazon.S3.Model;
using Application.AWS;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Application.Services
{
    public class AttachmentService : BaseService<Attachment>, IAttachmentService
    {
        private readonly IAmazonS3Service _amazonS3Service;
        private readonly IConfiguration _configuration;
        private readonly string? _connectionString;
        private readonly IAttachmentRepo _attachmentRepo;
        public AttachmentService(IAttachmentRepo attachmentRepo, IAmazonS3Service amazonS3Service, IConfiguration configuration) : base(attachmentRepo)
        {
            _amazonS3Service = amazonS3Service;
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
            _attachmentRepo = attachmentRepo;
        }

        public async Task<bool> UploadAttachment(IFormFile formFile, string prefix, Guid refId, RefType refType, Guid createdBy)
        {
            var random = RandomString();
            string fileKey = string.IsNullOrEmpty(prefix) ? $"{random}-{formFile.FileName}" : $"{prefix?.TrimEnd('/')}/{random}-{formFile.FileName}";
            var attachment = new Attachment();
            attachment.Id = Guid.NewGuid();
            attachment.Key = fileKey;
            attachment.RefId = refId;
            attachment.RefType = refType;
            attachment.CreatedBy = createdBy;
            var sqlConnection = new MySqlConnection(_connectionString);
            sqlConnection.Open();
            IDbTransaction dbTransaction = sqlConnection.BeginTransaction();
            try
            {
                var res = await Insert(attachment, sqlConnection, dbTransaction);
                if (res != 0)
                {
                    var resAws = await _amazonS3Service.UploadFile(formFile, fileKey, prefix);
                    if (!resAws)    
                    {
                        dbTransaction.Rollback();
                        sqlConnection.Close();
                        return false;
                    } else
                    {
                        dbTransaction.Commit();
                        sqlConnection.Close();
                        return true;
                    }
                }
                return false;

            }
            catch (Exception ex)
            {
                dbTransaction.Rollback();
                sqlConnection.Close();
                throw ex;
            }
        }

        private static string RandomString()
        {
            string path = Path.GetRandomFileName();
            path = path.Replace(".", "").Replace("/", ""); // Remove period.
            return path.Substring(0, 8);  // Return 8 character string
        }

        public async Task<List<string>> GetByRef(Guid refId, RefType refType)
        {
            var res = await _attachmentRepo.GetByRef(refId, refType);
            return res;
        }

        public async Task<bool> DeleteByKey(string fileKey)
        {
            var res = await _amazonS3Service.DeleteFileAsync(fileKey);
            if (res != null)
            {
                await _attachmentRepo.DeleteByField("Key", fileKey);
                return true;
            } else
            {
                return false;
            }
        }

        public async Task<GetObjectResponse> GetByKey(string key)
        {
            var s3Object = await _amazonS3Service.GetFileByKeyAsync(key);
            return s3Object;
        }
    }
}
