using Dm.util;
using Fastdotnet.Core.Dtos.System;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.Models.System;
using global::System.IO;
using global::System.IO.Compression;
using MailKit.Search;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using SqlSugar;
using System.Reflection;
using System.Text;
using System.Text.Json;
using static Npgsql.Replication.PgOutput.Messages.RelationMessage;

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
                    ColumnComment = column.ColumnDescription ?? column.DbColumnName,
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




        private string GeneratePropertyDefinition(FdCodeGenConfig column)
        {
            var attrStr = "";
            if (column.ColumnKey)
            {
                attrStr = $"[SugarColumn(IsPrimaryKey = true,ColumnName = \"{column.ColumnName.ToLower()}\", {GetLength(column.ColumnLength)} IsNullable = {column.WhetherRequired.ToString().ToLower()}, ColumnDescription = \"{column.ColumnComment}\" {GetDefaultValue(column.DefaultValue)})]";
            }
            else if (column.ColumnKey)
            {
                attrStr = $@"[SugarColumn(IsIdentity = true,ColumnName = ""{column.ColumnName.ToLower()}"", {GetLength(column.ColumnLength)} IsNullable = {column.WhetherRequired.ToString().ToLower()}, ColumnDescription = ""{column.ColumnComment}""{GetDefaultValue(column.DefaultValue)})]";
            }
            if (string.IsNullOrEmpty(attrStr))
            {
                attrStr += $@"[SugarColumn(ColumnName = ""{column.ColumnName.ToLower()}"", {GetLength(column.ColumnLength)} IsNullable = {column.WhetherRequired.ToString().ToLower()}, ColumnDescription = ""{column.ColumnComment}"" {GetDefaultValue(column.DefaultValue)})]";
            }
            return $"        {GenGenerateColumnComment(column.ColumnComment)}\n        {attrStr}\n        public {column.NetType} {column.PropertyName} {{ get; set; }}";
        }


        private string GetLength(int Length)
        {
            if (Length > 0)
            {
                return $"Length = {Length},";
            }
            else
            {
                return string.Empty;
            }
        }
        private string GetDefaultValue(string DefaultValue)
        {
            if (string.IsNullOrEmpty(DefaultValue))
            {
                return string.Empty;
            }
            else
            {
                return $",DefaultValue ={DefaultValue}";
            }
        }

        private string GenGenerateColumnComment(string ColumnComment)
        {
            return $@"
        /// <summary>
        /// {ColumnComment}
        /// </summary>";
        }


        private string GenerateDtoProperty(FdCodeGenConfig column, bool isCreate = false, bool isOutput = false)
        {
            var validations = new List<string>();
            if (!column.WhetherRequired && !column.ColumnKey && isCreate && column.NetType != "bool")
            {
                validations.Add("[Required(ErrorMessage = \"" + column.ShowColumnName + "不能为空\")]");
            }

            var lengthValidation = "";
            if (column.ColumnLength > 0 && (column.DataType?.Contains("char") == true || column.DataType?.Contains("text") == true))
            {
                if (!isOutput && column.NetType != "bool")
                {
                    lengthValidation = $"[StringLength({column.ColumnLength},ErrorMessage = \"{column.ShowColumnName}最多{column.ColumnLength}个字符\")]";
                }
                else
                {
                    if (column.EnableMask)
                    {
                        // 生成脱敏特性
                        var maskAttribute = GenerateMaskAttribute(column);
                        if (!string.IsNullOrEmpty(maskAttribute))
                        {
                            lengthValidation = maskAttribute;
                        }
                        else
                        {
                            lengthValidation = "        ";
                        }
                    }
                    else
                    {
                        lengthValidation = "        ";
                    }
                }
            }

            var validationStr = string.Join(" ", validations);
            if (!string.IsNullOrEmpty(lengthValidation))
            {
                validationStr = string.IsNullOrEmpty(validationStr) ? lengthValidation : $"{validationStr}\n        {lengthValidation}";
            }

            var result = $"{GenGenerateColumnComment(column.ShowColumnName)}\n";
            if (!string.IsNullOrEmpty(validationStr))
            {
                result += $"        {validationStr}\n        ";
            }
            else
            {
                result += $"        ";
            }
            result += $"public {column.NetType} {column.PropertyName} {{ get; set; }}";

            return result;
        }

        private string GenerateMaskAttribute(FdCodeGenConfig column)
        {
            // 检查是否启用脱敏以及是否有脱敏配置
            if (!column.EnableMask || column.MaskConfig == null)
                return string.Empty;

            try
            {
                // 反序列化脱敏配置
                var maskConfig = JsonConvert.DeserializeObject<MaskConfigModel>(column.MaskConfig);
                if (maskConfig?.Type == null)
                    return string.Empty;

                var attributeParams = new List<string>();

                // 根据脱敏类型添加参数
                switch (maskConfig.Type)
                {
                    case "Phone":
                        attributeParams.Add("SensitiveDataType.Phone");
                        break;
                    case "Email":
                        attributeParams.Add("SensitiveDataType.Email");
                        break;
                    case "IdCard":
                        attributeParams.Add("SensitiveDataType.IdCard");
                        break;
                    case "BankCard":
                        attributeParams.Add("SensitiveDataType.BankCard");
                        break;
                    case "Name":
                        attributeParams.Add("SensitiveDataType.Name");
                        break;
                    case "Custom":
                        attributeParams.Add("SensitiveDataType.Custom");
                        break;
                    default:
                        return string.Empty;
                }

                // 添加自定义参数
                var customParams = new List<string>();

                if (maskConfig.PrefixKeep.HasValue)
                    customParams.Add($"PrefixKeep = {maskConfig.PrefixKeep.Value}");

                if (maskConfig.SuffixKeep.HasValue)
                    customParams.Add($"SuffixKeep = {maskConfig.SuffixKeep.Value}");

                if (!string.IsNullOrEmpty(maskConfig.MaskChar.toString()) && maskConfig.MaskChar.toString() != "*")
                    customParams.Add($"MaskChar = '{maskConfig.MaskChar}'");

                if (maskConfig.MaskLength.HasValue)
                    customParams.Add($"MaskLength = {maskConfig.MaskLength.Value}");

                if (maskConfig.Type == "Custom")
                {
                    if (!string.IsNullOrEmpty(maskConfig.CustomPattern))
                        customParams.Add($"CustomPattern = \"{maskConfig.CustomPattern}\"");

                    if (!string.IsNullOrEmpty(maskConfig.CustomReplacement))
                        customParams.Add($"CustomReplacement = \"{maskConfig.CustomReplacement}\"");
                }

                // 组合所有参数
                if (customParams.Any())
                {
                    attributeParams.AddRange(customParams);
                }

                var paramsStr = string.Join(", ", attributeParams);
                return $"[SensitiveData({paramsStr})]";
            }
            catch
            {
                // 如果解析失败，返回空字符串
                return string.Empty;
            }
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
            if (isNullable)
            {
                netType += "?";
            }

            return netType;
        }


        public async Task<string> GenerateEntityContentAsync(string tableName, string entityName, List<FdCodeGenConfig> columns, string nameSpace, string TableComment)
        {
            // 使用反射获取BaseEntity的所有公共属性名称
            var baseEntityProperties = GetBaseEntityPropertyNames();

            var filteredColumns = columns.Where(col =>
                !baseEntityProperties.Contains(col.ColumnName, StringComparer.OrdinalIgnoreCase) &&
                !baseEntityProperties.Contains(col.PropertyName, StringComparer.OrdinalIgnoreCase)).ToList();

            return $@"using SqlSugar;

namespace {nameSpace ?? "Fastdotnet.Core.Entities"}
{{
    /// <summary>
    /// {TableComment} - 实体信息
    /// </summary>
    [SugarTable(""{tableName}"",""{TableComment}"")]
    public class {entityName} : BaseEntity
    {{
{string.Join("\n", filteredColumns.Select(col => GeneratePropertyDefinition(col)))}
    }}
}}";
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

        public async Task<string> GenerateDtoContentAsync(string entityName, List<FdCodeGenConfig> columns, string nameSpace, string TableComment)
        {
            // 使用反射获取BaseEntity的所有公共属性名称
            var baseEntityProperties = GetBaseEntityPropertyNames();

            var filteredColumns = columns.Where(col =>
                !baseEntityProperties.Contains(col.ColumnName, StringComparer.OrdinalIgnoreCase) &&
                !baseEntityProperties.Contains(col.PropertyName, StringComparer.OrdinalIgnoreCase)).ToList();
            var primaryKeyColumns = columns.Where(x => x.ColumnKey).ToList();
            primaryKeyColumns.AddRange(filteredColumns);

            var createDtoContent = $@"using System.ComponentModel.DataAnnotations;

namespace {nameSpace ?? "Fastdotnet.Core.Models"}
{{
    /// <summary>
    ///新增传输模型
    /// </summary>
    public class Create{entityName}Dto
    {{
{string.Join("\n", filteredColumns.Where(col => !col.ColumnKey && col.WhetherAddUpdate == true).Select(col => GenerateDtoProperty(col, true)))}
    }}

    /// <summary>
    ///修改传输模型
    /// </summary>
    public class Update{entityName}Dto
    {{
{string.Join("\n", primaryKeyColumns.Where(x => x.EnableMask == false && x.WhetherAddUpdate == true).Select(col => GenerateDtoProperty(col, false)))}
    }}

    /// <summary>
    ///输出传输模型
    /// </summary>
    public class {entityName}Dto
    {{
{string.Join("\n", columns.Where(x => x.WhetherTable == true).Select(col => GenerateDtoProperty(col, false, true)))}
    }}
}}";

            return $@"{createDtoContent}";
        }

        protected async Task<string> GenerateServiceInterfaceContent(string entityName, string nameSpace, string TableComment)
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

        public async Task<string> GenerateServiceImplementationContentAsync(string entityName, string nameSpace, string TableComment)
        {
            return $@"using Fastdotnet.Core.Entities;
using Fastdotnet.Core.IService;

namespace {nameSpace ?? "Fastdotnet.Service.Service"}
{{
    //除非要自定义自己的业务逻辑，否则不需要此文件
    public class {entityName}Service : BaseService<{entityName}>
    //, I{entityName}Service
    {{
        public {entityName}Service(IRepository<{entityName}> repository) : base(repository)
        {{
        }}
    }}
}}";
        }

        public async Task<string> GenerateControllerContentAsync(string entityName, string nameSpace, string TableComment)
        {
            return $@"using Fastdotnet.Core.Controllers;
using Fastdotnet.Core.Entities;
using Fastdotnet.Core.IService;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Fastdotnet.Core.Constants;
using Microsoft.AspNetCore.Mvc;

namespace {nameSpace ?? "Fastdotnet.WebApi.Controllers"}
{{
    /// <summary>
    /// {entityName} 控制器
    /// </summary>
    [Route(""api/[controller]"")]
    public class {entityName}Controller : GenericDtoControllerBase<{entityName}, string, Create{entityName}Dto, Update{entityName}Dto, {entityName}Dto>
    {{
        public {entityName}Controller(
            //I{entityName}Service {entityName.ToLower()}Service,
            IBaseService<{entityName}, string> service,
            IMapper mapper) : base(service, mapper)
        {{

        }}
    }}
}}";
        }


        public async Task<string> GenerateFrontendVueContentAsync(string entityName,  string busName, string pagePath, string TableComment, List<FdCodeGenConfig> configcolumns)
        {
            // 使用反射获取BaseEntity的所有公共属性名称
            var baseEntityProperties = GetBaseEntityPropertyNames();

            // 确保configcolumns不为null
            if (configcolumns == null)
                configcolumns = new List<FdCodeGenConfig>();

            //var filteredColumns = columns.Where(col =>
            //    !baseEntityProperties.Contains(col.ColumnName, StringComparer.OrdinalIgnoreCase) &&
            //    !baseEntityProperties.Contains(col.PropertyName, StringComparer.OrdinalIgnoreCase)).ToList();
            //var configcolumnsisshow = configcolumns.Where(x=>x.WhetherTable==true);
            var WhetherQuery = configcolumns.Where(x => x.WhetherQuery == true && x.QueryType == "BETWEEN") ?? new List<FdCodeGenConfig>();
            return $@"<template>
	<div class=""{entityName.ToLower()}-container"">
		<el-card shadow=""hover"" :body-style=""{{ padding: 2 }}"">
			<el-form :model=""state.queryParams"" ref=""queryForm"" :inline=""true"">
                <div v-show=""state.searchCollapsed"" >
                    {string.Join("\n\t", configcolumns.Where(x => x.WhetherQuery == true).Select((col, idx) =>
                                       $@"{getFrontendQueryTemp(col)}"

                    ))}
                </div>
				<el-form-item>
					<el-button-group>
						<el-button type=""primary"" icon=""ele-Search"" @click=""handleQuery""> 查询 </el-button>
						<el-button icon=""ele-Refresh"" @click=""resetQuery""> 重置 </el-button>
						<el-button @click=""toggleSearchCollapse"" 
							:icon=""state.searchCollapsed ? 'ele-ArrowUp' : 'ele-ArrowDown'"">
							{{{{ state.searchCollapsed ? '收起' : '展开' }}}}
						</el-button>
					</el-button-group>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card class=""full-table"" shadow=""hover"" style=""margin-top: 5px"">
			<div class=""table-toolbar"" style=""margin-bottom: 15px;"">
				<el-button type=""primary"" icon=""ele-Plus"" @click=""openAddDialog""> 新增 </el-button>
				<el-button icon=""ele-Download""> 导出 </el-button>
			</div>
			<el-table :data=""state.tableData.data"" style=""width: 100%"" v-loading=""state.loading"" border>
				{string.Join("\n\t\t\t\t", configcolumns.Where(x => x.WhetherTable == true).Select((col, idx) =>
                    $"				<el-table-column prop=\"{col.PropertyName}\" label=\"{col.ShowColumnName ?? col.PropertyName}\" show-overflow-tooltip />"
                ))}
				<el-table-column label=""操作"" width=""180"" fixed=""right"" align=""center"">
					<template #default=""scope"">
						<el-button icon=""ele-Edit"" size=""small"" text type=""primary"" @click=""openEditDialog(scope.row)"">修改</el-button>
						<el-button icon=""ele-Delete"" size=""small"" text type=""danger"" @click=""handleDelete(scope.row)"">删除</el-button>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination
				v-model:currentPage=""state.pagination.page""
				v-model:page-size=""state.pagination.pageSize""
				:total=""state.pagination.total""
				:page-sizes=""[10, 20, 50, 100]""
				size=""small""
				background
				@size-change=""handleSizeChange""
				@current-change=""handleCurrentChange""
				layout=""total, sizes, prev, pager, next, jumper""
			/>
		</el-card>

		<el-dialog v-model=""state.dialog.visible"" draggable :close-on-click-modal=""false"" width=""700px"">
			<template #header>
				<div style=""color: #fff"">
					<el-icon size=""16"" style=""margin-right: 3px; display: inline; vertical-align: middle""> <ele-Edit /> </el-icon>
					<span> {{ state.dialog.title }} </span>
				</div>
			</template>
			<el-form :model=""state.formData"" ref=""formRef"" label-width=""auto"">
				{string.Join("\n\t\t\t\t", configcolumns.Where(x => x.WhetherAddUpdate == true).Select(col =>
                    $"				<el-col :xs=\"24\" :sm=\"12\" :md=\"12\" :lg=\"12\" :xl=\"12\" class=\"mb20\">\n					<el-form-item label=\"{col.ShowColumnName ?? col.PropertyName}\" prop=\"{col.PropertyName}\">\n						{GetFormComponentByEffectType(col)}\n					</el-form-item>\n				</el-col>"
                ))}
			</el-form>
			<template #footer>
				<span class=""dialog-footer"">
					<el-button @click=""state.dialog.visible = false"">取 消</el-button>
					<el-button type=""primary"" @click=""submitForm"">确 定</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script lang=""ts"" setup name=""{entityName}"">
import {{ ref, reactive, onMounted }} from 'vue';
import {{ ElMessageBox, ElMessage }} from 'element-plus';
import {{buildMixedQuery}} from '@/utils/queryBuilder';

import dayjs from 'dayjs'; // 引入日期处理库
import * as {entityName}Api from '@/api/fd-system-api/{entityName}';

const queryForm = ref();
const formRef = ref();

const state = reactive({{
	loading: false,
    searchCollapsed: true, 
	tableData: {{
		data: [] as APIModel.{entityName}Dto[]
	}},
	queryParams: {{
	{string.Join("\n\t", configcolumns.Where(x => x.WhetherQuery == true).Select((col, idx) => $@"{col.PropertyName}:null,"))}
	{string.Join("\n\t", configcolumns.Where(x => x.WhetherQuery == true && x.QueryType == "BETWEEN").Select((col, idx) => $@"{col.PropertyName}_1:null,"))}
}},
	pagination: {{page: 1,
		pageSize: 20,
		total: 0,
	}},
	dialog: {{
		visible: false,
		title: '',
        type: 'create' as 'create' | 'update',
	}},
	formData: {{
    Id:'',
	{string.Join("\n\t", configcolumns.Where(x => x.WhetherAddUpdate == true).Select((col, idx) => $@"{col.PropertyName}:{getTSDefaultvAalue(col)},"))}
}}
}});
const toggleSearchCollapse = () => {{
	state.searchCollapsed = !state.searchCollapsed;
}};
// 获取列表
const getList = async () => {{
	state.loading = true;
    	try {{
	//构建查询条件
    const queryConfig = 
            {{
    {GetFrontendCondition(configcolumns)}
    }}
	const searchBody: APIModel.PageQueryByConditionDto = {{
			PageIndex: state.pagination.page,
			PageSize: state.pagination.pageSize,
	}};
	const queryResult = buildMixedQuery(queryConfig);
	if (queryResult.dynamicQuery) {{
		searchBody.DynamicQuery = queryResult.dynamicQuery;
		searchBody.QueryParameters = queryResult.queryParameters;
	}}
    // 调试日志
    //console.log('Search request body:', searchBody);
		const response = await {entityName}Api.postAdmin{entityName}PageSearch(searchBody);
		state.tableData.data = response.Items as APIModel.{entityName}Dto[] || [] as APIModel.{entityName}Dto[];
		state.pagination.total = response.PageInfo?.Total || 0;
	}} catch (error) {{
		ElMessage.error('获取数据失败');
		//console.error(error);
	}} finally {{
		state.loading = false;
	}}
}};

// 查询
const handleQuery = () => {{
	state.pagination.page = 1;
	getList();
}};

// 重置
const resetQuery = () => {{
	queryForm.value.resetFields();
	handleQuery();
}};

// 改变页面容量
const handleSizeChange = (val: number) => {{
	state.pagination.pageSize = val;
	getList();
}};

// 改变页码序号
const handleCurrentChange = (val: number) => {{
	state.pagination.page = val;
	getList();
}};

// 打开新增对话框
const openAddDialog = () => {{
	state.dialog.visible = true;
	state.dialog.title = '新增';
	state.dialog.type = 'create';
	formRef.value?.resetFields();
}};

// 打开编辑对话框
const openEditDialog = (row: any) => {{
	state.dialog.visible = true;
	state.dialog.title = '编辑';
	state.dialog.type = 'update';
	state.formData = {{ ...row }};
}};

// 提交表单
const submitForm = () => {{
	formRef.value.validate(async (valid: boolean) => {{
		if (!valid) return;
		try {{
			if (state.dialog.type === 'update'&&state.formData.{configcolumns.Where(w => w.ColumnKey == true).FirstOrDefault().PropertyName ?? configcolumns.FirstOrDefault().PropertyName}) {{
				// 更新接口调用
				const updateData = {{ ...state.formData }} as APIModel.Update{entityName}Dto;
				await {entityName}Api.putAdmin{entityName}Id({{ id: state.formData.Id }}, updateData);
				ElMessage.success('更新成功');
			}} else {{
				// 新增接口调用
                const createData= {{ ...state.formData }} as APIModel.Create{entityName}Dto;
                await {entityName}Api.postAdmin{entityName}(createData);
				ElMessage.success('添加成功');
			}}
			state.dialog.visible = false;
			getList();
		}} catch (error) {{
			console.error(error);
ElMessage.error(state.dialog.type === 'update' ? '更新失败' : '添加失败');
		}}
	}});
}};

// 删除
const handleDelete = (row: APIModel.{entityName}Dto) => {{
		ElMessageBox.confirm('确定删除吗？')
		.then(async () => {{
			// 删除接口调用
			await {entityName}Api.deleteAdmin{entityName}Id({{ id: row.Id as string }});
			ElMessage.success('删除成功');
			getList();
		}})
		.catch(() => {{
			ElMessage.error('删除失败');
			return;
		}});
}};

onMounted(() => {{
	getList();
}});
</script>
<style scoped lang=""scss"">
// .el-form--inline .el-form-item {{
// 	margin-right: 12px !important; // 稍微紧凑一点
// 	margin-bottom: 8px !important;
// }}

// .fdadminuser-container .el-card:first-child .el-form .el-form-item:last-of-type {{
// 	margin-bottom: 5 !important;
// }}
</style>


"


;
        }

        /// <summary>
        /// 获取前端最大输入值模板
        /// </summary>
        /// <returns></returns>
        private string getFrontendmMaxLenghtTemp(FdCodeGenConfig fdCodeGenConfig)
        {
            if (fdCodeGenConfig.ColumnLength > 0)
            {
                return $@"maxlength=""{fdCodeGenConfig.ColumnLength}"" show-word-limit";
            }
            
            return string.Empty;
        }


        private string GetFrontendCondition(List<FdCodeGenConfig> fdCodeGenConfig)
        {
            var ranges = new Dictionary<string, Dictionary<string, object>>();
            var jsObject = new global::System.Text.StringBuilder();
            jsObject.Append("ranges:{");

            foreach (var item in fdCodeGenConfig.Where(x => x.WhetherQuery == true && x.QueryType == "BETWEEN"))
            {
                jsObject.Append("\n" + $@"{item.PropertyName}:" + "{\n\tfrom:" + $@"state.queryParams.{item.PropertyName}," + "\n\tto:" + $@"state.queryParams.{item.PropertyName}_1" + "},\n");

            }
            jsObject.Append("}");
            var sb = new global::System.Text.StringBuilder();
            sb.Append("customs:[\n");
            foreach (var item in fdCodeGenConfig.Where(x => x.WhetherQuery == true && x.QueryType != "BETWEEN"))
            {
                sb.Append($@"{{field:'{item.PropertyName}',operator:'{item.QueryType}',value:state.queryParams.{item.PropertyName},}}," + "\n");
            }
            sb.Append("]");
            return sb.ToString() + ",\n" + jsObject.ToString();
        }

        private string getFrontendQueryTemp(FdCodeGenConfig fdCodeGenConfig)
        {
            if (fdCodeGenConfig.WhetherQuery == true && fdCodeGenConfig.QueryType == "BETWEEN")
            {
                // 对于BETWEEN查询，目前主要支持数值和日期类型的范围查询
                string componentStart, componentEnd;
                
                // 对于范围查询，我们假设是数值或日期类型
                if (fdCodeGenConfig.EffectType?.ToLower() == "datetime" || fdCodeGenConfig.EffectType?.ToLower() == "date" || fdCodeGenConfig.EffectType?.ToLower() == "datepicker" || fdCodeGenConfig.EffectType?.ToLower() == "time" || fdCodeGenConfig.EffectType?.ToLower() == "timepicker")
                {
                    componentStart = $@"<el-date-picker v-model=""state.queryParams.{fdCodeGenConfig.PropertyName}"" type=""datetime"" placeholder=""请选择起始{fdCodeGenConfig.ShowColumnName}"" clearable style=""width: 150px"" />";
                    componentEnd = $@"<el-date-picker v-model=""state.queryParams.{fdCodeGenConfig.PropertyName}_1"" type=""datetime"" placeholder=""请选择结束{fdCodeGenConfig.ShowColumnName}"" clearable style=""width: 150px"" />";
                }
                else
                {
                    // 默认为数值类型的范围查询
                    componentStart = $@"<el-input-number v-model=""state.queryParams.{fdCodeGenConfig.PropertyName}"" placeholder=""请输入起始{fdCodeGenConfig.ShowColumnName}"" style=""width: 150px"" />";
                    componentEnd = $@"<el-input-number v-model=""state.queryParams.{fdCodeGenConfig.PropertyName}_1"" placeholder=""请输入结束{fdCodeGenConfig.ShowColumnName}"" style=""width: 150px"" />";
                }
                
                return $@"<el-form-item label=""{fdCodeGenConfig.ShowColumnName}"" prop=""{fdCodeGenConfig.PropertyName}"">
                <div style=""display: flex; gap: 8px;"">
                    {componentStart}
                    <span style=""align-self: center;"">-</span>
                    {componentEnd}
                </div>
            </el-form-item>
            <el-form-item prop=""{fdCodeGenConfig.PropertyName}_1"" style=""display:none;""></el-form-item>";
            }

            else
            {
                // 根据EffectType生成查询组件
                var componentTemplate = GetQueryComponentByEffectType(fdCodeGenConfig);
                return $@"<el-form-item label=""{fdCodeGenConfig.ShowColumnName}"" prop=""{fdCodeGenConfig.PropertyName}"">
					{componentTemplate}
				</el-form-item>";
            }

        }

        /// <summary>
        /// 根据EffectType生成对应的查询组件模板
        /// </summary>
        /// <param name="fdCodeGenConfig">代码生成配置</param>
        /// <returns>组件模板字符串</returns>
        private string GetQueryComponentByEffectType(FdCodeGenConfig fdCodeGenConfig)
        {
            var effectType = fdCodeGenConfig.EffectType?.ToLower() ?? "input";
            var showColumnName = fdCodeGenConfig.ShowColumnName ?? fdCodeGenConfig.PropertyName;
            var isBoolType = fdCodeGenConfig.NetType?.ToLower() == "bool" || fdCodeGenConfig.NetType?.ToLower() == "bool?";

            return effectType switch
            {
                "input" or "text" => $@"<el-input v-model=""state.queryParams.{fdCodeGenConfig.PropertyName}"" placeholder=""请输入{showColumnName}"" clearable style=""width: 150px"" />",
                "select" or "dictselector" => $@"<el-select v-model=""state.queryParams.{fdCodeGenConfig.PropertyName}"" placeholder=""请选择{showColumnName}"" clearable style=""width: 150px"">
                    <el-option label=""选项1"" value=""1"" />
                    <el-option label=""选项2"" value=""2"" />
                </el-select>",
                "datetime" or "date" or "time" or "datepicker" or "timepicker" => $@"<el-date-picker v-model=""state.queryParams.{fdCodeGenConfig.PropertyName}"" type=""datetime"" placeholder=""请选择{showColumnName}"" clearable style=""width: 150px"" />",
                "numberinput" or "input-number" => $@"<el-input-number v-model=""state.queryParams.{fdCodeGenConfig.PropertyName}"" placeholder=""请输入{showColumnName}"" style=""width: 150px"" />",
                "switch" => $@"<el-select v-model=""state.queryParams.{fdCodeGenConfig.PropertyName}"" placeholder=""请选择{showColumnName}"" clearable style=""width: 150px"">
                    <el-option label=""是"" :value=""{GetBoolValue(fdCodeGenConfig, true)}"" />
                    <el-option label=""否"" :value=""{GetBoolValue(fdCodeGenConfig, false)}"" />
                </el-select>",
                "radio" => $@"<el-radio-group v-model=""state.queryParams.{fdCodeGenConfig.PropertyName}"" placeholder=""请选择{showColumnName}"" clearable style=""width: 150px"">
                    <el-radio :label=""{GetBoolValue(fdCodeGenConfig, true)}"">是</el-radio>
                    <el-radio :label=""{GetBoolValue(fdCodeGenConfig, false)}"">否</el-radio>
                </el-radio-group>",
                "checkbox" => $@"<el-checkbox-group v-model=""state.queryParams.{fdCodeGenConfig.PropertyName}"" placeholder=""请选择{showColumnName}"" clearable style=""width: 150px"">
                    <el-checkbox :label=""{GetBoolValue(fdCodeGenConfig, true)}"">是</el-checkbox>
                    <el-checkbox :label=""{GetBoolValue(fdCodeGenConfig, false)}"">否</el-checkbox>
                </el-checkbox-group>",
                "textarea" => $@"<el-input v-model=""state.queryParams.{fdCodeGenConfig.PropertyName}"" placeholder=""请输入{showColumnName}"" clearable style=""width: 150px"" type=""textarea"" />",
                _ => $@"<el-input v-model=""state.queryParams.{fdCodeGenConfig.PropertyName}"" placeholder=""请输入{showColumnName}"" clearable style=""width: 150px"" />"
            };
        }

        private string getTSDefaultvAalue(FdCodeGenConfig fdCodeGenConfig)
        {
            string a = "''";
            if (fdCodeGenConfig.NetType?.ToLower() == "bool" || fdCodeGenConfig.NetType?.ToLower() == "bool?")
            {
                a = "false";
            }
            else if (fdCodeGenConfig.NetType == "long")
            {
                a = "0";
            }
            return a;
        }

        /// <summary>
        /// 根据EffectType生成对应的表单组件模板
        /// </summary>
        /// <param name="fdCodeGenConfig">代码生成配置</param>
        /// <param name="columns">列信息</param>
        /// <returns>组件模板字符串</returns>
        private string GetFormComponentByEffectType(FdCodeGenConfig fdCodeGenConfig)
        {
            var effectType = fdCodeGenConfig.EffectType?.ToLower() ?? "input";
            var showColumnName = fdCodeGenConfig.ShowColumnName ?? fdCodeGenConfig.PropertyName;
            var maxLengthAttr = getFrontendmMaxLenghtTemp(fdCodeGenConfig);
            var isBoolType = fdCodeGenConfig.NetType?.ToLower() == "bool" || fdCodeGenConfig.NetType?.ToLower() == "bool?";

            return effectType switch
            {
                "input" or "text" => $@"<el-input v-model=""state.formData.{fdCodeGenConfig.PropertyName}"" placeholder=""请输入{showColumnName}"" {maxLengthAttr} clearable />",
                "select" or "dictselector" => $@"<el-select v-model=""state.formData.{fdCodeGenConfig.PropertyName}"" placeholder=""请选择{showColumnName}"" {maxLengthAttr} clearable style=""width: 100%"">
                    <el-option label=""选项1"" value=""1"" />
                    <el-option label=""选项2"" value=""2"" />
                </el-select>",
                "datetime" or "date" or "time" or "datepicker" or "timepicker" => $@"<el-date-picker v-model=""state.formData.{fdCodeGenConfig.PropertyName}"" type=""datetime"" placeholder=""请选择{showColumnName}"" {maxLengthAttr} style=""width: 100%"" value-format=""YYYY-MM-DD HH:mm:ss"" />",
                "numberinput" or "input-number" => $@"<el-input-number v-model=""state.formData.{fdCodeGenConfig.PropertyName}"" placeholder=""请输入{showColumnName}"" {maxLengthAttr} style=""width: 100%"" />",
                "switch" => $@"<el-switch v-model=""state.formData.{fdCodeGenConfig.PropertyName}"" :active-value=""{GetBoolValue(fdCodeGenConfig, true)}"" :inactive-value=""{GetBoolValue(fdCodeGenConfig, false)}"" />",
                "radio" => $@"<el-radio-group v-model=""state.formData.{fdCodeGenConfig.PropertyName}"">
                    <el-radio :label=""{GetBoolValue(fdCodeGenConfig, true)}"">是</el-radio>
                    <el-radio :label=""{GetBoolValue(fdCodeGenConfig, false)}"">否</el-radio>
                </el-radio-group>",
                "checkbox" => $@"<el-checkbox-group v-model=""state.formData.{fdCodeGenConfig.PropertyName}"">
                    <el-checkbox :label=""{GetBoolValue(fdCodeGenConfig, true)}"">是</el-checkbox>
                    <el-checkbox :label=""{GetBoolValue(fdCodeGenConfig, false)}"">否</el-checkbox>
                </el-checkbox-group>",
                "textarea" => $@"<el-input v-model=""state.formData.{fdCodeGenConfig.PropertyName}"" placeholder=""请输入{showColumnName}"" {maxLengthAttr} type=""textarea"" rows=""4"" />",
                "password" => $@"<el-input v-model=""state.formData.{fdCodeGenConfig.PropertyName}"" placeholder=""请输入{showColumnName}"" {maxLengthAttr} type=""password"" show-password />",
                "upload" => $@"<el-upload
                    v-model:file-list=""state.formData.{fdCodeGenConfig.PropertyName}""
                    class=""upload-demo""
                    action=""/api/upload""
                    multiple
                    :limit=""3"">
                    <el-button size=""small"" type=""primary"">点击上传</el-button>
                    <template #tip>
                        <div class=""el-upload__tip"">支持多文件上传，最多3个文件</div>
                    </template>
                </el-upload>",
                _ => $@"<el-input v-model=""state.formData.{fdCodeGenConfig.PropertyName}"" placeholder=""请输入{showColumnName}"" {maxLengthAttr} clearable />"
            };
        }
        
        /// <summary>
        /// 根据字段类型获取布尔值的表示形式
        /// </summary>
        /// <param name="fdCodeGenConfig">代码生成配置</param>
        /// <param name="boolValue">布尔值</param>
        /// <returns>布尔值的字符串表示</returns>
        private string GetBoolValue(FdCodeGenConfig fdCodeGenConfig, bool boolValue)
        {
            var isBoolType = fdCodeGenConfig.NetType?.ToLower() == "bool" || fdCodeGenConfig.NetType?.ToLower() == "bool?";
            return isBoolType ? (boolValue ? "true" : "false") : (boolValue ? "1" : "0");
        }

        /// <summary>
        /// 根据字段名和数据类型推断EffectType
        /// 规范：
        /// - 包含 'status' 字眼 → 'select' (下拉框)
        /// - 包含 'type' 字眼 → 'select' (下拉框) 
        /// - 包含 'time'/'date'/'create'/'update' 字眼 → 'datetime' (日期时间选择器)
        /// - 包含 'image'/'img' 字眼 → 'upload' (上传组件)
        /// - 包含 'file' 字眼 → 'upload' (上传组件)
        /// - 包含 'desc'/'detail'/'content' 字眼 → 'textarea' (文本域)
        /// - 包含 'url'/'link' 字眼 → 'url' (链接输入框)
        /// - 包含 'email' 字眼 → 'email' (邮箱输入框)
        /// - 包含 'phone'/'tel'/'mobile' 字眼 → 'phone' (电话输入框)
        /// - 包含 'password' 字眼 → 'password' (密码框)
        /// - 包含 'gender'/'sex' 字眼 → 'radio' (单选框)
        /// - 包含 'tags'/'category' 字眼 → 'checkbox' (多选框)
        /// - 包含 'dict' 字眼 → 'dict' (字典选择)
        /// - 长度 > 200 的字符串 → 'textarea' (文本域)
        /// - 其他情况 → 根据数据类型决定
        /// </summary>
        /// <param name="columnName">字段名</param>
        /// <param name="dataType">数据类型</param>
        /// <returns>推断的作用类型</returns>
        public string GetEffectTypeByColumnName(string columnName, string dataType)
        {
            if (string.IsNullOrEmpty(columnName))
                return GetEffectTypeByDataType(dataType);

            columnName = columnName.ToLower();

            // 根据字段名推断
            if (columnName.Contains("status")) return "Select";
            if (columnName.Contains("type")) return "Select";
            if (columnName.Contains("time") || columnName.Contains("Date") ||
                columnName.Contains("create") || columnName.Contains("update") ||
                columnName.Contains("modify") || columnName.Contains("gmt_") || columnName.Contains("_at")) return "DatePicker";
            if (columnName.Contains("image") || columnName.Contains("img")) return "Upload";
            if (columnName.Contains("file")) return "Upload";
            if (columnName.Contains("desc") || columnName.Contains("detail") ||
                columnName.Contains("content") || columnName.Contains("memo") ||
                columnName.Contains("note")) return "textarea";
            if (columnName.Contains("url") || columnName.Contains("link")) return "Input";
            if (columnName.Contains("email")) return "Input";
            if (columnName.Contains("phone") || columnName.Contains("Input") ||
                columnName.Contains("mobile")) return "Input";
            if (columnName.Contains("password")) return "Input";
            if (columnName.Contains("gender") || columnName.Contains("sex")) return "Radio";
            if (columnName.Contains("tags") || columnName.Contains("category") ||
                columnName.Contains("role")) return "checkbox";
            if (columnName.Contains("dict")) return "dict";

            // 根据数据类型推断（如果字段名没有明确含义）
            return GetEffectTypeByDataType(dataType);
        }


        /// <summary>
        /// 根据数据类型推断EffectType
        /// </summary>
        /// <param name="dataType">数据类型</param>
        /// <returns>作用类型</returns>
        private static string GetEffectTypeByDataType(string dataType)
        {
            if (string.IsNullOrEmpty(dataType))
                return "Input";

            dataType = dataType.ToLower();

            // 数值类型
            if (dataType.Contains("int") || dataType.Contains("decimal") ||
                dataType.Contains("double") || dataType.Contains("float") ||
                dataType.Contains("tiny") || dataType.Contains("small") ||
                dataType.Contains("big") || dataType.Contains("number"))
                return "NumberInput";

            // 日期时间类型
            if (dataType.Contains("date") || dataType.Contains("time") ||
                dataType.Contains("timestamp"))
                return "DatePicker";

            // 布尔类型
            if (dataType.Contains("bool") || dataType.Contains("bit"))
                return "Switch";

            // 文本类型（长度较长）
            if (dataType.Contains("text") || dataType.Contains("memo"))
                return "Textarea";

            // 默认为普通输入框
            return "Input";
        }
    }
}