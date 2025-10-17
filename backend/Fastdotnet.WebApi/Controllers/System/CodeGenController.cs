using AutoMapper;
using Fastdotnet.Core.Constants;
using Fastdotnet.Core.Controllers;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Exceptions;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.Models.System;
using Fastdotnet.Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;

namespace Fastdotnet.WebApi.Controllers.System
{
    /// <summary>
    /// 代码生成相关接口
    /// </summary>
    [Route("api/[controller]")]
    public class CodeGenController : GenericDtoControllerBase<CodeGenConfig, string, CreateCodeGenConfigDto, UpdateCodeGenConfigDto, CodeGenConfigDto>
    {
        private readonly ICodeGenConfigService _codeGenConfigService;

        public CodeGenController(
            ICodeGenConfigService codeGenConfigService,
            IRepository<CodeGenConfig> repository,
            IMapper mapper) : base(repository, mapper)
        {
            _codeGenConfigService = codeGenConfigService;
        }
        protected override async Task BeforeCreate(CodeGenConfig entity, CreateCodeGenConfigDto dto)
        {
            if (string.IsNullOrEmpty(entity.EntityName))
            {
                entity.EntityName = _codeGenConfigService.GetEntityNameByTableName(entity.TableName);
            }
            await base.BeforeCreate(entity, dto);
        }
        /// <summary>
        /// 获取库表数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("gettablelist")]
        [Authorize(Policy = Permissions.System.CodeGen.View)]
        public async Task<List<TableInfoDto>> GetTableList()
        {
            return await _codeGenConfigService.GetTableListAsync();
        }

        /// <summary>
        /// 获取表的列数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("gettablecolumnlist")]
        [Authorize(Policy = Permissions.System.CodeGen.View)]
        public async Task<List<ColumnInfoDto>> GetTableColumnList(string TableName)
        {
            if (string.IsNullOrEmpty(TableName))
            {
                throw new ArgumentException("表名不能为空");
            }
            return await _codeGenConfigService.GetTableColumnListAsync(TableName);
        }

        /// <summary>
        /// 根据表名获取实体名
        /// </summary>
        /// <returns></returns>
        [HttpGet("getentityname")]
        [Authorize(Policy = Permissions.System.CodeGen.View)]
        public async Task<string> GetEntityName(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentException("表名不能为空");
            }
            return _codeGenConfigService.GetEntityNameByTableName(tableName);
        }

        /// <summary>
        /// 执行代码生成
        /// </summary>
        /// <returns></returns>
        [HttpPost("generate")]
        [Authorize(Policy = Permissions.System.CodeGen.Create)]
        public async Task<string> GenerateCode([FromBody] CodeGenInput input)
        {
            if (string.IsNullOrEmpty(input.TableName))
            {
                throw new ArgumentException("表名不能为空");
            }
            return await _codeGenConfigService.GenerateCodeAsync(input);
        }


        /// <summary>
        /// 根据配置ID获取表列表
        /// </summary>
        /// <param name="configId">配置ID</param>
        /// <returns></returns>
        [HttpGet("tablelist/{configId}")]
        [Authorize(Policy = Permissions.System.CodeGen.View)]
        public async Task<List<TableInfoDto>> GetTableListByConfigId(string configId)
        {
            return await _codeGenConfigService.GetTableListAsync();
        }

        /// <summary>
        /// 根据表名和配置ID获取列信息
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="configId">配置ID</param>
        /// <returns></returns>
        [HttpGet("columnlist/{tableName}/{configId}")]
        [Authorize(Policy = Permissions.System.CodeGen.View)]
        public async Task<List<ColumnInfoDto>> GetColumnListByTableNameAndConfigId(string tableName, string configId)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentException("表名不能为空");
            }
            return await _codeGenConfigService.GetTableColumnListAsync(tableName);
        }

        /// <summary>
        /// 获取应用命名空间列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("applicationnamespaces")]
        [Authorize(Policy = Permissions.System.CodeGen.View)]
        public async Task<List<string>> GetApplicationNamespaces()
        {
            // 模拟返回应用命名空间
            return new List<string>
            {
                "Fastdotnet.Service",
                "Fastdotnet.Core",
                "Fastdotnet.WebApi",
                "Fastdotnet.Plugin"
            };
        }

        /// <summary>
        /// 下载生成的代码文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        [HttpGet("download")]
        [Authorize(Policy = Permissions.System.CodeGen.Create)]
        public async Task<IActionResult> DownloadFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return BadRequest("文件路径不能为空");
            }

            var fullPath = Path.GetFullPath(filePath);
            
            // 确保路径在临时目录中以防止路径遍历攻击
            var tempPath = Path.GetTempPath();
            if (!fullPath.StartsWith(tempPath, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("非法的文件路径");
            }

            if (!global::System.IO.File.Exists(fullPath))
            {
                return NotFound("文件不存在");
            }

            var fileName = Path.GetFileName(fullPath);
            var fileStream = global::System.IO.File.OpenRead(fullPath);
            
            return File(fileStream, "application/zip", fileName);
        }
    }
}
