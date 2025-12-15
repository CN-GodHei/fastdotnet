namespace Fastdotnet.WebApi.Controllers.System
{
    /// <summary>
    /// 代码生成相关接口
    /// </summary>
    [Route("api/[controller]")]
    public class CodeGenController : GenericDtoControllerBase<FdCodeGen, string, CreateCodeGenDto, UpdateCodeGenDto, CodeGenConfigDto>
    {
        private readonly ICodeGenConfigService _codeGenConfigService;
        private readonly IBaseService<FdCodeGen, string> _service;
        private readonly IRepository<FdCodeGenConfig> _configRepository;

        public CodeGenController(
            ICodeGenConfigService codeGenConfigService,
            IBaseService<FdCodeGen, string> service,
            IRepository<FdCodeGenConfig> configRepository,
            IMapper mapper) : base(service, mapper)
        {
            _codeGenConfigService = codeGenConfigService;
            _service = service;
            _configRepository = configRepository;
        }
        protected override async Task BeforeCreate(FdCodeGen entity, CreateCodeGenDto dto)
        {
            var record = await _service.GetListAsync(w => w.TableName == dto.TableName);
            if (record.Count > 0)
            {
                throw new BusinessException("该表已生成!");
            }

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
        //[HttpPost("generate")]
        //[Authorize(Policy = Permissions.System.CodeGen.Create)]
        //public async Task<string> GenerateCode([FromBody] CodeGenInput input)
        //{
        //    if (string.IsNullOrEmpty(input.TableName))
        //    {
        //        throw new ArgumentException("表名不能为空");
        //    }
        //    return await _codeGenConfigService.GenerateCodeAsync(input,"");
        //}


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
        /// 预览生成的代码
        /// </summary>
        /// <param name="configId">代码生成配置ID</param>
        /// <param name="type">代码类型：entity, dto, service, controller, frontend</param>
        /// <returns></returns>
        [HttpGet("preview/{configId}")]
        [Authorize(Policy = Permissions.System.CodeGen.View)]
        public async Task<string> PreviewCode(string configId, [FromQuery] string type = "entity")
        {
            if (string.IsNullOrEmpty(configId))
            {
                throw new ArgumentException("配置ID不能为空");
            }

            // 获取配置信息
            var config = await _service.GetByIdAsync(configId);
            if (config == null)
            {
                throw new ArgumentException("配置不存在");
            }

            var entityName = _codeGenConfigService.GetEntityNameByTableName(config.TableName);
            //List<ColumnInfoDto>? tableColumns = await _codeGenConfigService.GetTableColumnListAsync(config.TableName);
            List<FdCodeGenConfig>? tableColumnsconfig = await _configRepository.GetListAsync(w=>w.CodeGenId== configId);
            //tableColumns.ForEach(x =>
            //{
            //    x.ShowColumnName = tableColumnsconfig.FirstOrDefault(s => s.ColumnName == x.ColumnName)?.ShowColumnName ?? x.PropertyName;
            //});

            // 根据类型生成不同的代码
            return type?.ToLower() switch
            {
                "entity" => await _codeGenConfigService.GenerateEntityContentAsync(config.TableName, entityName, tableColumnsconfig, config.NameSpace, config.TableComment),
                "dto" => await _codeGenConfigService.GenerateDtoContentAsync(entityName, tableColumnsconfig, config.NameSpace, config.TableComment),
                "service" => await _codeGenConfigService.GenerateServiceImplementationContentAsync(entityName, config.NameSpace, config.TableComment),
                "controller" => await _codeGenConfigService.GenerateControllerContentAsync(entityName, config.NameSpace, config.TableComment),
                "frontend" => await _codeGenConfigService.GenerateFrontendVueContentAsync(entityName,  config.TableName, config.PagePath, config.TableComment, tableColumnsconfig),
                _ => await _codeGenConfigService.GenerateEntityContentAsync(config.TableName, entityName, tableColumnsconfig, config.NameSpace, config.TableComment) // 默认返回实体代码
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

        protected override async Task AfterCreate(FdCodeGen entity, CreateCodeGenDto dto)
        {
            var tableColumns = await _codeGenConfigService.GetTableColumnListAsync(entity.TableName);
            var baseEntityProperties = GetBaseEntityPropertyNames();
            // 将表列信息转换为 FdCodeGenConfig 实体列表并保存
            if (tableColumns != null && tableColumns.Any())
            {
                // 转换并插入新的配置数据
                var configList = tableColumns.Select((col, index) => new FdCodeGenConfig
                {
                    CodeGenId = entity.Id, // 直接使用字符串ID
                    ColumnName = col.ColumnName,
                    ShowColumnName = col.ColumnComment??col.PropertyName,
                    ColumnKey = col.IsPrimarykey, // 主键标识
                    PropertyName = col.PropertyName,
                    ColumnLength = col.Length,
                    ColumnComment = col.ColumnComment,
                    DataType = col.DataType,
                    NetType = col.NetType,
                    DefaultValue = col.DefaultValue,
                    EffectType = _codeGenConfigService.GetEffectTypeByColumnName(col.ColumnName, col.DataType), // 根据字段名和数据类型推断作用类型
                    OrderNo = index + 100, // 默认排序
                    WhetherTable = baseEntityProperties.Contains(col.PropertyName) ? false : true,
                    WhetherAddUpdate = baseEntityProperties.Contains(col.PropertyName) ? false : true,
                    WhetherImport = baseEntityProperties.Contains(col.PropertyName) ? false : true,
                    WhetherSortable = baseEntityProperties.Contains(col.PropertyName) ? false : true,
                    WhetherQuery = baseEntityProperties.Contains(col.PropertyName) ? false : true,
                    QueryType = baseEntityProperties.Contains(col.PropertyName) ? "" : "eq",//一般来说检索是Like合适但是，这个是批量生成的，避免所有查询都like影响性能，直接按等于生成，可以自己在页面上调整
                    WhetherRequired= col.IsNullable? false : true,
                    EnableMask = false,
                }).ToList();

                await _configRepository.InsertRangeAsync(configList);
            }

            await base.AfterCreate(entity, dto);
        }





        protected override async Task AfterDelete(string id, bool result)
        {
            await _configRepository.DeleteAsync(w => w.CodeGenId == id);
            await base.AfterDelete(id, result);
        }
        private HashSet<string> GetBaseEntityPropertyNames()
        {
            var baseEntityProperties = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var properties = typeof(Core.Models.Base.BaseEntity).GetProperties();
            foreach (var property in properties)
            {
                baseEntityProperties.Add(property.Name);
            }

            return baseEntityProperties;
        }

    }
}
