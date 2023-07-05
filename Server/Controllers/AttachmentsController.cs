using Application.AWS;
using Application.Services;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttachmentsController : BaseController<Attachment>
    {
        private readonly IAttachmentService _attachmentService;

        public AttachmentsController(IAttachmentService attachmentService) : base(attachmentService)
        {
            _attachmentService = attachmentService;
        }

        [HttpGet("aws")]
        public async Task<IActionResult> Get(string key)
        {
            try
            {
                var s3Object = await _attachmentService.GetByKey(key);
                if (s3Object == null) return NotFound();
                return File(s3Object.ResponseStream, s3Object.Headers.ContentType);

            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("file-key")]
        public async Task<IActionResult> GetByRef(Guid refId, RefType refType)
        {
            try
            {
                var res = await _attachmentService.GetByRef(refId, refType);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("aws")]
        public async Task<IActionResult> UploadFileAsync(IFormFile file, string prefix, Guid refId, RefType refType, Guid createdBy)
        {
            try
            {
                var res = await _attachmentService.UploadAttachment(file, prefix, refId, refType, createdBy);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        
        }

        [HttpDelete("aws")]
        public async Task<IActionResult> DeleteFile(string key)
        {
            try
            {
                var res = await _attachmentService.DeleteByKey(key);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
