using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.Models.System;
using SqlSugar;
using global::System.IO;
using global::System.IO.Compression;

namespace Fastdotnet.Service.Service
{
    public class CodeGenConfigService : ICodeGenConfigService
    {
        protected readonly IRepository<FdCodeGen> _repository;
        protected readonly ISqlSugarClient _db;

        public CodeGenConfigService(IRepository<FdCodeGen> repository, ISqlSugarClient db)
        {
            _repository = repository;
            _db = db;
        }

        public async Task<List<TableInfoDto>> GetTableListAsync()
        {
            var db = _db;
            var tables = db.DbMaintenance.GetTableInfoList(false);
            
            var result = new List<TableInfoDto>();
            foreach (var table in tables)
            {
                result.Add(new TableInfoDto
                {
                    TableName = table.Name,
                    EntityName = GetEntityNameByTableName(table.Name),
                    TableComment = table.Description ?? table.Name
                });
            }

            return result;
        }

        public async Task<List<ColumnInfoDto>> GetTableColumnListAsync(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentException("表名不能为空");
            }

            var db = _db;
            var columns = db.DbMaintenance.GetColumnInfosByTableName(tableName, false);

            var result = new List<ColumnInfoDto>();
            foreach (var column in columns)
            {
                result.Add(new ColumnInfoDto
                {
                    ColumnName = column.DbColumnName,
                    PropertyName = ToPascalCase(column.DbColumnName),
                    DataType = column.DataType,
                    NetType = GetNetType(column.DataType, column.IsNullable),
                    IsPrimarykey = column.IsPrimarykey,
                    IsIdentity = column.IsIdentity,
                    IsNullable = column.IsNullable,
                    Length = column.Length,
                    Scale = column.Scale,
                    DefaultValue = column.DefaultValue,
                    ColumnComment = column.ColumnDescription ?? column.DbColumnName
                });
            }

            return result;
        }

        public string GetEntityNameByTableName(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
                return string.Empty;

            // 移除表前缀（如：sys_、tb_、t_）
            var cleanName = tableName;
            if (tableName.StartsWith("sys_", StringComparison.OrdinalIgnoreCase))
                cleanName = tableName.Substring(4);
            else if (tableName.StartsWith("tb_", StringComparison.OrdinalIgnoreCase))
                cleanName = tableName.Substring(3);
            else if (tableName.StartsWith("t_", StringComparison.OrdinalIgnoreCase))
                cleanName = tableName.Substring(2);

            // 转换为 PascalCase
            var parts = cleanName.Split('_');
            var entityName = string.Join("", parts.Select(part => 
                char.ToUpper(part.FirstOrDefault()) + (part.Length > 1 ? part.Substring(1).ToLower() : "")));
            
            // 确保首字母大写
            if (!string.IsNullOrEmpty(entityName))
            {
                entityName = char.ToUpper(entityName[0]) + entityName.Substring(1);
            }

            return entityName;
        }

        public async Task<string> GenerateCodeAsync(CodeGenInput input)
        {
            var tableColumns = await GetTableColumnListAsync(input.TableName ?? string.Empty);
            var entityName = GetEntityNameByTableName(input.TableName ?? string.Empty);
            
            // 创建临时目录用于生成代码文件
            var tempDir = global::System.IO.Path.Combine(global::System.IO.Path.GetTempPath(), $"codegen_{input.TableName}_{DateTime.Now:yyyyMMddHHmmss}");
            if (!global::System.IO.Directory.Exists(tempDir))
            {
                global::System.IO.Directory.CreateDirectory(tempDir);
            }

            try
            {
                // 生成实体类
                await GenerateEntityFile(tempDir, input.TableName, entityName, tableColumns, input.NameSpace);
                
                // 生成DTO类
                await GenerateDtoFiles(tempDir, entityName, tableColumns, input.NameSpace);
                
                // 生成服务接口
                await GenerateServiceInterfaceFile(tempDir, entityName, input.NameSpace);
                
                // 生成服务实现
                await GenerateServiceImplementationFile(tempDir, entityName, input.NameSpace);
                
                // 生成控制器
                await GenerateControllerFile(tempDir, entityName, input.NameSpace, "Developer"); // 使用默认作者名
                
                // 生成前端页面
                await GenerateFrontendFiles(tempDir, entityName, tableColumns, input.TableName, input.PagePath); // 使用表名代替业务名

                // 压缩为ZIP文件
                var zipPath = global::System.IO.Path.Combine(global::System.IO.Path.GetTempPath(), $"codegen_{input.TableName}_{DateTime.Now:yyyyMMddHHmmss}.zip");
                if (global::System.IO.File.Exists(zipPath))
                {
                    global::System.IO.File.Delete(zipPath);
                }
                global::System.IO.Compression.ZipFile.CreateFromDirectory(tempDir, zipPath);

                // 返回ZIP文件路径
                return zipPath;
            }
            finally
            {
                // 清理临时目录
                if (global::System.IO.Directory.Exists(tempDir))
                {
                    global::System.IO.Directory.Delete(tempDir, true);
                }
            }
        }

        private async Task GenerateEntityFile(string outputDir, string tableName, string entityName, List<ColumnInfoDto> columns, string nameSpace)
        {
            var entityDir = global::System.IO.Path.Combine(outputDir, "Entity");
            if (!global::System.IO.Directory.Exists(entityDir))
            {
                global::System.IO.Directory.CreateDirectory(entityDir);
            }

            var entityContent = $@"using SqlSugar;

namespace {nameSpace ?? "Fastdotnet.Core.Entities"}
{{
    /// <summary>
    /// {entityName} - {tableName}
    /// </summary>
    [SugarTable(""{tableName}"")]
    public class {entityName} : BaseEntity
    {{
{string.Join("\n", columns.Select(col => GeneratePropertyDefinition(col)))}
    }}
}}";

            var entityPath = global::System.IO.Path.Combine(entityDir, $"{entityName}.cs");
            await global::System.IO.File.WriteAllTextAsync(entityPath, entityContent);
        }

        private string GeneratePropertyDefinition(ColumnInfoDto column)
        {
            var attrStr = "";
            if (column.IsPrimarykey)
            {
                attrStr = "[SugarColumn(IsPrimaryKey = true)]\n        ";
            }
            else if (column.IsIdentity)
            {
                attrStr = "[SugarColumn(IsIdentity = true)]\n        ";
            }
            
            return $"        {attrStr}public {column.NetType} {column.PropertyName} {{ get; set; }}";
        }

        private async Task GenerateDtoFiles(string outputDir, string entityName, List<ColumnInfoDto> columns, string nameSpace)
        {
            var dtoDir = global::System.IO.Path.Combine(outputDir, "Dto");
            if (!global::System.IO.Directory.Exists(dtoDir))
            {
                global::System.IO.Directory.CreateDirectory(dtoDir);
            }

            // Create DTO
            var createDtoContent = $@"using System.ComponentModel.DataAnnotations;

namespace {nameSpace ?? "Fastdotnet.Core.Models"}
{{
    public class Create{entityName}Dto
    {{
{string.Join("\n", columns.Where(col => !col.IsPrimarykey && !col.IsIdentity).Select(col => GenerateDtoProperty(col, true)))}
    }}
}}";

            // Update DTO
            var updateDtoContent = $@"using System.ComponentModel.DataAnnotations;

namespace {nameSpace ?? "Fastdotnet.Core.Models"}
{{
    public class Update{entityName}Dto
    {{
{string.Join("\n", columns.Where(col => !col.IsPrimarykey && !col.IsIdentity).Select(col => GenerateDtoProperty(col, false)))}
    }}
}}";

            // Output DTO
            var outputDtoContent = $@"namespace {nameSpace ?? "Fastdotnet.Core.Models"}
{{
    public class {entityName}Dto
    {{
{string.Join("\n", columns.Select(col => GenerateDtoProperty(col, false)))}
    }}
}}";

            var dtoPath = global::System.IO.Path.Combine(dtoDir, $"{entityName}Dto.cs");
            var fullContent = $@"{createDtoContent}

{updateDtoContent}

{outputDtoContent}
";
            await global::System.IO.File.WriteAllTextAsync(dtoPath, fullContent);
        }

        private string GenerateDtoProperty(ColumnInfoDto column, bool isCreate = false)
        {
            var validations = new List<string>();
            if (!column.IsNullable && !column.IsPrimarykey && !column.IsIdentity && isCreate)
            {
                validations.Add("[Required]");
            }

            var lengthValidation = "";
            if (column.Length > 0 && (column.DataType?.Contains("char") == true || column.DataType?.Contains("text") == true))
            {
                lengthValidation = $"[StringLength({column.Length})]";
            }

            var validationStr = string.Join(" ", validations);
            if (!string.IsNullOrEmpty(lengthValidation))
            {
                validationStr = string.IsNullOrEmpty(validationStr) ? lengthValidation : $"{validationStr} {lengthValidation}";
            }

            var result = "";
            if (!string.IsNullOrEmpty(validationStr))
            {
                result += $"        {validationStr}\n        ";
            }

            result += $"public {column.NetType} {column.PropertyName} {{ get; set; }}";
            
            return result;
        }

        private async Task GenerateServiceInterfaceFile(string outputDir, string entityName, string nameSpace)
        {
            var serviceDir = global::System.IO.Path.Combine(outputDir, "ServiceInterface");
            if (!Directory.Exists(serviceDir))
            {
                global::System.IO.Directory.CreateDirectory(serviceDir);
            }

            var serviceInterfaceContent = $@"using Fastdotnet.Core.Entities;

namespace {nameSpace ?? "Fastdotnet.Core.IService"}
{{
    public interface I{entityName}Service : IBaseService<{entityName}>
    {{
        // 在这里添加自定义业务方法
    }}
}}";

            var serviceInterfacePath = global::System.IO.Path.Combine(serviceDir, $"I{entityName}Service.cs");
            await global::System.IO.File.WriteAllTextAsync(serviceInterfacePath, serviceInterfaceContent);
        }

        private async Task GenerateServiceImplementationFile(string outputDir, string entityName, string nameSpace)
        {
            var serviceDir = global::System.IO.Path.Combine(outputDir, "ServiceImplementation");
            if (!Directory.Exists(serviceDir))
            {
                global::System.IO.Directory.CreateDirectory(serviceDir);
            }

            var serviceImplementationContent = $@"using Fastdotnet.Core.Entities;
using Fastdotnet.Core.IService;

namespace {nameSpace ?? "Fastdotnet.Service.Service"}
{{
    public class {entityName}Service : BaseService<{entityName}>, I{entityName}Service
    {{
        public {entityName}Service(IRepository<{entityName}> repository) : base(repository)
        {{
        }}
    }}
}}";

            var serviceImplementationPath = global::System.IO.Path.Combine(serviceDir, $"{entityName}Service.cs");
            await global::System.IO.File.WriteAllTextAsync(serviceImplementationPath, serviceImplementationContent);
        }

        private async Task GenerateControllerFile(string outputDir, string entityName, string nameSpace, string authorName)
        {
            var controllerDir = global::System.IO.Path.Combine(outputDir, "Controller");
            if (!Directory.Exists(controllerDir))
            {
                global::System.IO.Directory.CreateDirectory(controllerDir);
            }

            var controllerContent = $@"using Fastdotnet.Core.Controllers;
using Fastdotnet.Core.Entities;
using Fastdotnet.Core.IService;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Fastdotnet.Core.Constants;

namespace {nameSpace ?? "Fastdotnet.WebApi.Controllers"}
{{
    /// <summary>
    /// {entityName} 控制器
    /// </summary>
    [Route(""api/[controller]"")]
    public class {entityName}Controller : GenericDtoControllerBase<{entityName}, string, Create{entityName}Dto, Update{entityName}Dto, {entityName}Dto>
    {{
        public {entityName}Controller(
            I{entityName}Service {entityName.ToLower()}Service,
            IRepository<{entityName}> repository,
            IMapper mapper) : base(repository, mapper)
        {{
        }}
    }}
}}";

            var controllerPath = global::System.IO.Path.Combine(controllerDir, $"{entityName}Controller.cs");
            await global::System.IO.File.WriteAllTextAsync(controllerPath, controllerContent);
        }

        private async Task GenerateFrontendFiles(string outputDir, string entityName, List<ColumnInfoDto> columns, string busName, string pagePath)
        {
            var frontendDir = global::System.IO.Path.Combine(outputDir, "Frontend");
            if (!Directory.Exists(frontendDir))
            {
                global::System.IO.Directory.CreateDirectory(frontendDir);
            }

            // 创建页面目录
            var pageDir = global::System.IO.Path.Combine(frontendDir, pagePath ?? "default");
            if (!Directory.Exists(pageDir))
            {
                global::System.IO.Directory.CreateDirectory(pageDir);
            }

            // 创建Vue页面
            var vueContent = $@"<template>
	<div class=""{entityName.ToLower()}-container"">
		<el-card shadow=""hover"" :body-style=""{{ padding: 5 }}"">
			<el-form :model=""queryParams"" ref=""queryForm"" :inline=""true"">
				<el-form-item label=""搜索条件"">
					<el-input placeholder=""请输入搜索条件"" clearable @keyup.enter=""handleQuery"" v-model=""queryParams.searchValue"" />
				</el-form-item>
				<el-form-item>
					<el-button-group>
						<el-button type=""primary"" icon=""ele-Search"" @click=""handleQuery""> 查询 </el-button>
						<el-button icon=""ele-Refresh"" @click=""resetQuery""> 重置 </el-button>
					</el-button-group>
				</el-form-item>
				<el-form-item>
					<el-button type=""primary"" icon=""ele-Plus"" @click=""openAddDialog""> 新增 </el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card class=""full-table"" shadow=""hover"" style=""margin-top: 5px"">
			<el-table :data=""tableData"" style=""width: 100%"" v-loading=""loading"" border>
				{string.Join("\n\t\t\t\t", columns.Take(3).Select((col, idx) => 
					$"				<el-table-column prop=\"{col.PropertyName}\" label=\"{col.ColumnComment ?? col.PropertyName}\" align=\"center\" show-overflow-tooltip />"
				))}
				<el-table-column label=""操作"" width=""180"" fixed=""right"" align=""center"">
					<template #default=""scope"">
						<el-button icon=""ele-Edit"" size=""small"" text type=""primary"" @click=""openEditDialog(scope.row)"">修改</el-button>
						<el-button icon=""ele-Delete"" size=""small"" text type=""danger"" @click=""handleDelete(scope.row)"">删除</el-button>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination
				v-model:currentPage=""tableParams.page""
				v-model:page-size=""tableParams.pageSize""
				:total=""tableParams.total""
				:page-sizes=""[10, 20, 50, 100]""
				size=""small""
				background
				@size-change=""handleSizeChange""
				@current-change=""handleCurrentChange""
				layout=""total, sizes, prev, pager, next, jumper""
			/>
		</el-card>

		<el-dialog v-model=""dialog.visible"" draggable :close-on-click-modal=""false"" width=""700px"">
			<template #header>
				<div style=""color: #fff"">
					<el-icon size=""16"" style=""margin-right: 3px; display: inline; vertical-align: middle""> <ele-Edit /> </el-icon>
					<span> {{ dialog.title }} </span>
				</div>
			</template>
			<el-form :model=""formData"" ref=""formRef"" label-width=""auto"">
				{string.Join("\n\t\t\t\t", columns.Where(col => !col.IsPrimarykey && !col.IsIdentity).Take(5).Select(col => 
					$"				<el-col :xs=\"24\" :sm=\"12\" :md=\"12\" :lg=\"12\" :xl=\"12\" class=\"mb20\">\n					<el-form-item label=\"{col.ColumnComment ?? col.PropertyName}\" prop=\"{col.PropertyName}\">\n						<el-input v-model=\"formData.{col.PropertyName}\" placeholder=\"请输入{col.ColumnComment ?? col.PropertyName}\" clearable />\n					</el-form-item>\n				</el-col>"
				))}
			</el-form>
			<template #footer>
				<span class=""dialog-footer"">
					<el-button @click=""dialog.visible = false"">取 消</el-button>
					<el-button type=""primary"" @click=""submitForm"">确 定</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script lang=""ts"" setup name=""{entityName}"">
import {{ ref, reactive, onMounted }} from 'vue';
import {{ ElMessageBox, ElMessage }} from 'element-plus';

const queryForm = ref();
const formRef = ref();

const state = reactive({{
	loading: false,
	tableData: [],
	queryParams: {{
		searchValue: undefined
	}},
	tableParams: {{
		page: 1,
		pageSize: 20,
		total: 0
	}},
	dialog: {{
		visible: false,
		title: ''
	}},
	formData: {{}}
}});

// 获取列表
const getList = async () => {{
	state.loading = true;
	// TODO: 实现获取列表接口调用
	state.loading = false;
}};

// 查询
const handleQuery = () => {{
	state.tableParams.page = 1;
	getList();
}};

// 重置
const resetQuery = () => {{
	queryForm.value.resetFields();
	handleQuery();
}};

// 改变页面容量
const handleSizeChange = (val: number) => {{
	state.tableParams.pageSize = val;
	getList();
}};

// 改变页码序号
const handleCurrentChange = (val: number) => {{
	state.tableParams.page = val;
	getList();
}};

// 打开新增对话框
const openAddDialog = () => {{
	state.dialog.visible = true;
	state.dialog.title = '新增{busName}';
	formRef.value?.resetFields();
	state.formData = {{}};
}};

// 打开编辑对话框
const openEditDialog = (row: any) => {{
	state.dialog.visible = true;
	state.dialog.title = '编辑{busName}';
	state.formData = {{ ...row }};
}};

// 提交表单
const submitForm = () => {{
	formRef.value.validate(async (valid: boolean) => {{
		if (!valid) return;
		try {{
			if (state.formData.id) {{
				// 更新接口调用
				ElMessage.success('更新成功');
			}} else {{
				// 新增接口调用
				ElMessage.success('新增成功');
			}}
			state.dialog.visible = false;
			getList();
		}} catch (error) {{
			console.error(error);
		}}
	}});
}};

// 删除
const handleDelete = (row: any) => {{
	ElMessageBox.confirm('确定删除吗？')
		.then(async () => {{
			// 删除接口调用
			ElMessage.success('删除成功');
			getList();
		}})
		.catch(() => {{}});
}};

onMounted(() => {{
	getList();
}});
</script>
";
            var vuePath = global::System.IO.Path.Combine(pageDir, $"{entityName}.vue");
            await global::System.IO.File.WriteAllTextAsync(vuePath, vueContent);
        }

        private string ToPascalCase(string columnName)
        {
            if (string.IsNullOrEmpty(columnName))
                return string.Empty;

            var parts = columnName.Split('_');
            return string.Join("", parts.Select(part =>
                char.ToUpper(part.FirstOrDefault()) + (part.Length > 1 ? part.Substring(1).ToLower() : "")
            ));
        }

        private string GetNetType(string dbType, bool isNullable)
        {
            string netType = dbType.ToLower() switch
            {
                "int" or "integer" => "int",
                "bigint" => "long",
                "smallint" => "short",
                "tinyint" => "byte",
                "bit" or "boolean" => "bool",
                "varchar" or "char" or "text" or "longtext" or "mediumtext" or "ntext" or "nchar" or "nvarchar" => "string",
                "decimal" or "numeric" or "money" or "smallmoney" => "decimal",
                "float" or "real" => "float",
                "double" => "double",
                "datetime" or "datetime2" or "timestamp" or "date" or "time" => "DateTime",
                "uniqueidentifier" or "guid" => "Guid",
                "varbinary" or "binary" or "image" or "blob" or "mediumblob" or "longblob" => "byte[]",
                _ => "string"
            };

            // 如果可空且非 string 类型，追加 ?
            if (isNullable && netType != "string")
            {
                netType += "?";
            }

            return netType;
        }

        public async Task<List<PreviewFileItem>> GetGeneratedFileListAsync(CodeGenInput input)
        {
            var tableColumns = await GetTableColumnListAsync(input.TableName ?? string.Empty);
            var entityName = GetEntityNameByTableName(input.TableName ?? string.Empty);
            
            var fileList = new List<PreviewFileItem>();
            
            // 实体类
            fileList.Add(new PreviewFileItem 
            { 
                Name = $"{entityName}.cs", 
                Path = $"src/{input.NameSpace}/Entities/{entityName}.cs", 
                Type = "cs" 
            });
            
            // DTO类
            fileList.Add(new PreviewFileItem 
            { 
                Name = $"{entityName}Dto.cs", 
                Path = $"src/{input.NameSpace}/Dtos/{entityName}Dto.cs", 
                Type = "cs" 
            });
            
            // 服务接口
            fileList.Add(new PreviewFileItem 
            { 
                Name = $"I{entityName}Service.cs", 
                Path = $"src/{input.NameSpace}/IService/I{entityName}Service.cs", 
                Type = "cs" 
            });
            
            // 服务实现
            fileList.Add(new PreviewFileItem 
            { 
                Name = $"{entityName}Service.cs", 
                Path = $"src/{input.NameSpace}/Service/{entityName}Service.cs", 
                Type = "cs" 
            });
            
            // 控制器
            fileList.Add(new PreviewFileItem 
            { 
                Name = $"{entityName}Controller.cs", 
                Path = $"src/{input.NameSpace}/Controllers/{entityName}Controller.cs", 
                Type = "cs" 
            });
            
            // 前端API
            fileList.Add(new PreviewFileItem 
            { 
                Name = $"{entityName?.ToLower()}-api.ts", 
                Path = $"src/api/{input.TableName}-api.ts", 
                Type = "ts" 
            });
            
            // 前端页面
            fileList.Add(new PreviewFileItem 
            { 
                Name = $"{entityName}.vue", 
                Path = $"src/views/{input.PagePath}/{entityName?.ToLower()}/index.vue", 
                Type = "vue" 
            });
            
            return fileList;
        }

        public async Task<string> GetGeneratedFileContentAsync(CodeGenInput input, string filePath)
        {
            var tableColumns = await GetTableColumnListAsync(input.TableName ?? string.Empty);
            var entityName = GetEntityNameByTableName(input.TableName ?? string.Empty);
            
            // 这里我们根据文件路径返回对应的生成内容
            if (filePath.EndsWith($"{entityName}.cs"))
            {
                return await GenerateEntityContent(input.TableName, entityName, tableColumns, input.NameSpace);
            }
            else if (filePath.EndsWith($"{entityName}Dto.cs"))
            {
                return await GenerateDtoContent(entityName, tableColumns, input.NameSpace);
            }
            else if (filePath.EndsWith($"I{entityName}Service.cs"))
            {
                return await GenerateServiceInterfaceContent(entityName, input.NameSpace);
            }
            else if (filePath.EndsWith($"{entityName}Service.cs"))
            {
                return await GenerateServiceImplementationContent(entityName, input.NameSpace);
            }
            else if (filePath.EndsWith($"{entityName}Controller.cs"))
            {
                return await GenerateControllerContent(entityName, input.NameSpace, "Developer"); // 使用默认作者名
            }
            else if (filePath.EndsWith("-api.ts"))
            {
                return await GenerateFrontendApiContent(entityName, input.TableName); // 使用表名代替业务名
            }
            else if (filePath.EndsWith(".vue"))
            {
                return await GenerateFrontendVueContent(entityName, tableColumns, input.TableName, input.PagePath); // 使用表名代替业务名
            }
            
            return "// 文件不存在或无法生成预览内容";
        }

        public async Task<CodeGenPreviewResult> PreviewCodeAsync(CodeGenInput input)
        {
            var previewResult = new CodeGenPreviewResult
            {
                FileList = await GetGeneratedFileListAsync(input)
            };
            
            return previewResult;
        }

        protected async Task<string> GenerateEntityContent(string tableName, string entityName, List<ColumnInfoDto> columns, string nameSpace)
        {
            return $@"using SqlSugar;

namespace {nameSpace ?? "Fastdotnet.Core.Entities"}
{{
    /// <summary>
    /// {entityName} - {tableName}
    /// </summary>
    [SugarTable(""{tableName}"")]
    public class {entityName} : BaseEntity
    {{
{string.Join("\n", columns.Select(col => GeneratePropertyDefinition(col)))}
    }}
}}";
        }

        protected async Task<string> GenerateDtoContent(string entityName, List<ColumnInfoDto> columns, string nameSpace)
        {
            // Create DTO
            var createDtoContent = $@"using System.ComponentModel.DataAnnotations;

namespace {nameSpace ?? "Fastdotnet.Core.Models"}
{{
    public class Create{entityName}Dto
    {{
{string.Join("\n", columns.Where(col => !col.IsPrimarykey && !col.IsIdentity).Select(col => GenerateDtoProperty(col, true)))}
    }}
}}";

            // Update DTO
            var updateDtoContent = $@"using System.ComponentModel.DataAnnotations;

namespace {nameSpace ?? "Fastdotnet.Core.Models"}
{{
    public class Update{entityName}Dto
    {{
{string.Join("\n", columns.Where(col => !col.IsPrimarykey && !col.IsIdentity).Select(col => GenerateDtoProperty(col, false)))}
    }}
}}";

            // Output DTO
            var outputDtoContent = $@"namespace {nameSpace ?? "Fastdotnet.Core.Models"}
{{
    public class {entityName}Dto
    {{
{string.Join("\n", columns.Select(col => GenerateDtoProperty(col, false)))}
    }}
}}";

            return $@"{createDtoContent}

{updateDtoContent}

{outputDtoContent}";
        }

        protected async Task<string> GenerateServiceInterfaceContent(string entityName, string nameSpace)
        {
            return $@"using Fastdotnet.Core.Entities;

namespace {nameSpace ?? "Fastdotnet.Core.IService"}
{{
    public interface I{entityName}Service : IBaseService<{entityName}>
    {{
        // 在这里添加自定义业务方法
    }}
}}";
        }

        protected async Task<string> GenerateServiceImplementationContent(string entityName, string nameSpace)
        {
            return $@"using Fastdotnet.Core.Entities;
using Fastdotnet.Core.IService;

namespace {nameSpace ?? "Fastdotnet.Service.Service"}
{{
    public class {entityName}Service : BaseService<{entityName}>, I{entityName}Service
    {{
        public {entityName}Service(IRepository<{entityName}> repository) : base(repository)
        {{
        }}
    }}
}}";
        }

        protected async Task<string> GenerateControllerContent(string entityName, string nameSpace, string authorName = "Developer")
        {
            return $@"using Fastdotnet.Core.Controllers;
using Fastdotnet.Core.Entities;
using Fastdotnet.Core.IService;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Fastdotnet.Core.Constants;

namespace {nameSpace ?? "Fastdotnet.WebApi.Controllers"}
{{
    /// <summary>
    /// {entityName} 控制器
    /// </summary>
    [Route(""api/[controller]"")]
    public class {entityName}Controller : GenericDtoControllerBase<{entityName}, string, Create{entityName}Dto, Update{entityName}Dto, {entityName}Dto>
    {{
        public {entityName}Controller(
            I{entityName}Service {entityName.ToLower()}Service,
            IRepository<{entityName}> repository,
            IMapper mapper) : base(repository, mapper)
        {{
        }}
    }}
}}";
        }

        private async Task<string> GenerateFrontendApiContent(string entityName, string busName)
        {
            return $@"// @ts-ignore
/* eslint-disable */
import request from '/@/utils/request';

/** 获取所有记录 检索并返回系统中该类型的所有记录。 GET /api/{entityName} */
export async function get{entityName}List(options?: {{ [key: string]: any }}) {{
	return request<{entityName}Dto[]>('/api/{entityName}', {{
		method: 'GET',
		...(options || {{}}),
	}});
}}

/** 创建新记录 根据提供的数据创建一条新记录。 POST /api/{entityName} */
export async function post{entityName}(body: Create{entityName}Dto, options?: {{ [key: string]: any }}) {{
	return request<{entityName}Dto>('/api/{entityName}', {{
		method: 'POST',
		headers: {{
			'Content-Type': 'application/json-patch+json',
		}},
		data: body,
		...(options || {{}}),
	}});
}}

/** 根据ID获取记录 根据提供的唯一标识符(ID)检索特定记录的详细信息。 GET /api/{entityName}/${{param0}} */
export async function get{entityName}ById(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: get{entityName}ByIdParams,
	options?: {{ [key: string]: any }}
) {{
	const {{ id: param0, ...queryParams }} = params;
	return request<{entityName}Dto>(`/api/{entityName}/${{param0}}`, {{
		method: 'GET',
		params: {{ ...queryParams }},
		...(options || {{}}),
	}});
}}

/** 更新现有记录 根据提供的ID和更新数据，修改现有记录的信息。 PUT /api/{entityName}/${{param0}} */
export async function put{entityName}ById(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: put{entityName}ByIdParams,
	body: Update{entityName}Dto,
	options?: {{ [key: string]: any }}
) {{
	const {{ id: param0, ...queryParams }} = params;
	return request<{entityName}Dto>(`/api/{entityName}/${{param0}}`, {{
		method: 'PUT',
		headers: {{
			'Content-Type': 'application/json-patch+json',
		}},
		params: {{ ...queryParams }},
		data: body,
		...(options || {{}}),
	}});
}}

/** 删除记录 根据提供的ID，从系统中移除指定的记录。 DELETE /api/{entityName}/${{param0}} */
export async function delete{entityName}ById(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: delete{entityName}ByIdParams,
	options?: {{ [key: string]: any }}
) {{
	const {{ id: param0, ...queryParams }} = params;
	return request<boolean>(`/api/{entityName}/${{param0}}`, {{
		method: 'DELETE',
		params: {{ ...queryParams }},
		...(options || {{}}),
	}});
}}

/** 分页获取记录 根据页码和页面大小，分页检索记录。 GET /api/{entityName}/page */
export async function get{entityName}Page(
	// 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
	params: get{entityName}PageParams,
	options?: {{ [key: string]: any }}
) {{
	return request<{entityName}DtoPageResult>('/api/{entityName}/page', {{
		method: 'GET',
		params: {{
			// pageIndex has a default value: 1
			pageIndex: '1',
			// pageSize has a default value: 10
			pageSize: '10',
			...params,
		}},
		...(options || {{}}),
	}});
}}";
        }

        public async Task<string> GenerateEntityContentAsync(string tableName, string entityName, List<ColumnInfoDto> columns, string nameSpace)
        {
            return await GenerateEntityContent(tableName, entityName, columns, nameSpace);
        }

        public async Task<string> GenerateDtoContentAsync(string entityName, List<ColumnInfoDto> columns, string nameSpace)
        {
            return await GenerateDtoContent(entityName, columns, nameSpace);
        }

        public async Task<string> GenerateServiceImplementationContentAsync(string entityName, string nameSpace)
        {
            return await GenerateServiceImplementationContent(entityName, nameSpace);
        }

        public async Task<string> GenerateControllerContentAsync(string entityName, string nameSpace)
        {
            return await GenerateControllerContent(entityName, nameSpace, "Developer"); // 使用默认作者名
        }

        public async Task<string> GenerateFrontendVueContentAsync(string entityName, List<ColumnInfoDto> columns, string tableName, string pagePath)
        {
            return await GenerateFrontendVueContent(entityName, columns, tableName, pagePath);
        }

        private async Task<string> GenerateFrontendVueContent(string entityName, List<ColumnInfoDto> columns, string busName, string pagePath)
        {
            return $@"<template>
	<div class=""{entityName.ToLower()}-container"">
		<el-card shadow=""hover"" :body-style=""{{ padding: 5 }}"">
			<el-form :model=""queryParams"" ref=""queryForm"" :inline=""true"">
				<el-form-item label=""搜索条件"">
					<el-input placeholder=""请输入搜索条件"" clearable @keyup.enter=""handleQuery"" v-model=""queryParams.searchValue"" />
				</el-form-item>
				<el-form-item>
					<el-button-group>
						<el-button type=""primary"" icon=""ele-Search"" @click=""handleQuery""> 查询 </el-button>
						<el-button icon=""ele-Refresh"" @click=""resetQuery""> 重置 </el-button>
					</el-button-group>
				</el-form-item>
				<el-form-item>
					<el-button type=""primary"" icon=""ele-Plus"" @click=""openAddDialog""> 新增 </el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card class=""full-table"" shadow=""hover"" style=""margin-top: 5px"">
			<el-table :data=""tableData"" style=""width: 100%"" v-loading=""loading"" border>
				{string.Join("\n\t\t\t\t", columns.Take(3).Select((col, idx) => 
                    $"				<el-table-column prop=\"{col.PropertyName}\" label=\"{col.ColumnComment ?? col.PropertyName}\" align=\"center\" show-overflow-tooltip />"
                ))}
				<el-table-column label=""操作"" width=""180"" fixed=""right"" align=""center"">
					<template #default=""scope"">
						<el-button icon=""ele-Edit"" size=""small"" text type=""primary"" @click=""openEditDialog(scope.row)"">修改</el-button>
						<el-button icon=""ele-Delete"" size=""small"" text type=""danger"" @click=""handleDelete(scope.row)"">删除</el-button>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination
				v-model:currentPage=""tableParams.page""
				v-model:page-size=""tableParams.pageSize""
				:total=""tableParams.total""
				:page-sizes=""[10, 20, 50, 100]""
				size=""small""
				background
				@size-change=""handleSizeChange""
				@current-change=""handleCurrentChange""
				layout=""total, sizes, prev, pager, next, jumper""
			/>
		</el-card>

		<el-dialog v-model=""dialog.visible"" draggable :close-on-click-modal=""false"" width=""700px"">
			<template #header>
				<div style=""color: #fff"">
					<el-icon size=""16"" style=""margin-right: 3px; display: inline; vertical-align: middle""> <ele-Edit /> </el-icon>
					<span> {{ dialog.title }} </span>
				</div>
			</template>
			<el-form :model=""formData"" ref=""formRef"" label-width=""auto"">
				{string.Join("\n\t\t\t\t", columns.Where(col => !col.IsPrimarykey && !col.IsIdentity).Take(5).Select(col => 
                    $"				<el-col :xs=\"24\" :sm=\"12\" :md=\"12\" :lg=\"12\" :xl=\"12\" class=\"mb20\">\n					<el-form-item label=\"{col.ColumnComment ?? col.PropertyName}\" prop=\"{col.PropertyName}\">\n						<el-input v-model=\"formData.{col.PropertyName}\" placeholder=\"请输入{col.ColumnComment ?? col.PropertyName}\" clearable />\n					</el-form-item>\n				</el-col>"
                ))}
			</el-form>
			<template #footer>
				<span class=""dialog-footer"">
					<el-button @click=""dialog.visible = false"">取 消</el-button>
					<el-button type=""primary"" @click=""submitForm"">确 定</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script lang=""ts"" setup name=""{entityName}"">
import {{ ref, reactive, onMounted }} from 'vue';
import {{ ElMessageBox, ElMessage }} from 'element-plus';

const queryForm = ref();
const formRef = ref();

const state = reactive({{
	loading: false,
	tableData: [],
	queryParams: {{
		searchValue: undefined
	}},
	tableParams: {{
		page: 1,
		pageSize: 20,
		total: 0
	}},
	dialog: {{
		visible: false,
		title: ''
	}},
	formData: {{}}
}});

// 获取列表
const getList = async () => {{
	state.loading = true;
	// TODO: 实现获取列表接口调用
	state.loading = false;
}};

// 查询
const handleQuery = () => {{
	state.tableParams.page = 1;
	getList();
}};

// 重置
const resetQuery = () => {{
	queryForm.value.resetFields();
	handleQuery();
}};

// 改变页面容量
const handleSizeChange = (val: number) => {{
	state.tableParams.pageSize = val;
	getList();
}};

// 改变页码序号
const handleCurrentChange = (val: number) => {{
	state.tableParams.page = val;
	getList();
}};

// 打开新增对话框
const openAddDialog = () => {{
	state.dialog.visible = true;
	state.dialog.title = '新增{busName}';
	formRef.value?.resetFields();
	state.formData = {{}};
}};

// 打开编辑对话框
const openEditDialog = (row: any) => {{
	state.dialog.visible = true;
	state.dialog.title = '编辑{busName}';
	state.formData = {{ ...row }};
}};

// 提交表单
const submitForm = () => {{
	formRef.value.validate(async (valid: boolean) => {{
		if (!valid) return;
		try {{
			if (state.formData.id) {{
				// 更新接口调用
				ElMessage.success('更新成功');
			}} else {{
				// 新增接口调用
				ElMessage.success('新增成功');
			}}
			state.dialog.visible = false;
			getList();
		}} catch (error) {{
			console.error(error);
		}}
	}});
}};

// 删除
const handleDelete = (row: any) => {{
	ElMessageBox.confirm('确定删除吗？')
		.then(async () => {{
			// 删除接口调用
			ElMessage.success('删除成功');
			getList();
		}})
		.catch(() => {{}});
}};

onMounted(() => {{
	getList();
}});
</script>";
        }
    }
}