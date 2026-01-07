using Fastdotnet.Core.IService.Sys;
using Fastdotnet.Core.Service.Sys;
using Microsoft.AspNetCore.StaticFiles;

namespace Fastdotnet.WebApi.Controllers
{
    /// <summary>
    /// 存储控制器，提供文件上传下载功能
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [ApiUsageScope(Core.Enum.ApiUsageScopeEnum.AdminOnly)]
    public class StorageController : ControllerBase
    {
        private readonly IStorageService _storageService;
        private readonly IWebHostEnvironment _environment;
        private readonly StorageOptions _storageOptions;

        public StorageController(IStorageService storageService, IWebHostEnvironment environment, IOptions<StorageOptions> storageOptions)
        {
            _storageService = storageService;
            _environment = environment;
            _storageOptions = storageOptions.Value;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="file">要上传的文件</param>
        /// <param name="bucketName">存储桶名称（可选）</param>
        /// <returns>上传结果</returns>
        [HttpPost("upload")]
        [AllowAnonymous]
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
        /// 通过URL直接访问上传的文件（公共访问接口）
        /// </summary>
        /// <param name="relativePath">文件相对路径</param>
        /// <returns>文件内容</returns>
        [AllowAnonymous]
        [HttpGet("/uploads/{*relativePath}")]
        public async Task<ActionResult> GetFileByPath(string relativePath)
        {
            try
            {
                // 构建实际的文件路径
                var filePath = Path.Combine(_environment.ContentRootPath, _storageOptions.LocalStoragePath, relativePath);
                
                // 验证路径安全性，防止路径遍历攻击
                var baseDirectory = Path.GetFullPath(Path.Combine(_environment.ContentRootPath, _storageOptions.LocalStoragePath));
                var fullFilePath = Path.GetFullPath(filePath);
                
                if (!fullFilePath.StartsWith(baseDirectory, StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest("非法路径访问");
                }

                if (!global::System.IO.File.Exists(fullFilePath))
                {
                    return NotFound("文件不存在");
                }

                var fileExtension = Path.GetExtension(fullFilePath);
                var contentType = GetContentType(fileExtension);

                // 对于图片文件，设置适当的头部以支持渐进式渲染
                if (IsImageFile(fileExtension))
                {
                    Response.Headers.Add("Accept-Ranges", "bytes");
                    Response.Headers.Add("Cache-Control", "public, max-age=31536000"); // 缓存一年
                }

                // 使用PhysicalFile，它会自动处理范围请求
                return PhysicalFile(fullFilePath, contentType);
            }
            catch (Exception ex)
            {
                return BadRequest($"访问文件时发生错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 检查是否为图片文件
        /// </summary>
        /// <param name="fileExtension">文件扩展名</param>
        /// <returns>是否为图片文件</returns>
        private bool IsImageFile(string fileExtension)
        {
            var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", ".tiff", ".svg" };
            return imageExtensions.Contains(fileExtension.ToLower());
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