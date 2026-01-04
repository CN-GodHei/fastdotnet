
using Fastdotnet.Core.IService.Sys;

namespace Fastdotnet.WebApi.Controllers
{
    /// <summary>
    /// 存储控制器，提供文件上传下载功能
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class StorageController : ControllerBase
    {
        private readonly IStorageService _storageService;

        public StorageController(IStorageService storageService)
        {
            _storageService = storageService;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="file">要上传的文件</param>
        /// <param name="bucketName">存储桶名称（可选）</param>
        /// <returns>上传结果</returns>
        [HttpPost("upload")]
        public async Task<ActionResult<string>> UploadAsync(IFormFile file, string? bucketName = null)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("文件不能为空");
            }

            using var stream = file.OpenReadStream();
            var url = await _storageService.UploadAsync(stream, file.FileName, bucketName);

            return Ok(new { Url = url, FileName = file.FileName });
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="bucketName">存储桶名称（可选）</param>
        /// <returns>文件内容</returns>
        [HttpGet("download/{fileName}")]
        public async Task<ActionResult> DownloadAsync(string fileName, string? bucketName = null)
        {
            try
            {
                var fileData = await _storageService.DownloadAsync(fileName, bucketName);
                var fileExtension = global::System.IO.Path.GetExtension(fileName);
                var contentType = GetContentType(fileExtension);

                return File(fileData, contentType);
            }
            catch (global::System.IO.FileNotFoundException)
            {
                return NotFound("文件不存在");
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="bucketName">存储桶名称（可选）</param>
        /// <returns>删除结果</returns>
        [HttpDelete("delete/{fileName}")]
        public async Task<ActionResult<bool>> DeleteAsync(string fileName, string? bucketName = null)
        {
            var result = await _storageService.DeleteAsync(fileName, bucketName);
            return Ok(new { Success = result });
        }

        /// <summary>
        /// 获取文件URL
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="bucketName">存储桶名称（可选）</param>
        /// <returns>文件URL</returns>
        [HttpGet("url/{fileName}")]
        public async Task<ActionResult<string>> GetFileUrlAsync(string fileName, string? bucketName = null)
        {
            var url = await _storageService.GetFileUrlAsync(fileName, bucketName);
            return Ok(new { Url = url });
        }

        /// <summary>
        /// 根据文件扩展名获取Content-Type
        /// </summary>
        /// <param name="fileExtension">文件扩展名</param>
        /// <returns>Content-Type</returns>
        private string GetContentType(string fileExtension)
        {
            return fileExtension.ToLower() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".pdf" => "application/pdf",
                ".txt" => "text/plain",
                ".html" => "text/html",
                ".css" => "text/css",
                ".js" => "application/javascript",
                ".json" => "application/json",
                ".xml" => "application/xml",
                _ => "application/octet-stream"
            };
        }
    }
}