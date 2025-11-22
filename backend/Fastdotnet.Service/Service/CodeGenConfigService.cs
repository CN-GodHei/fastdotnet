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




        private string GeneratePropertyDefinition(ColumnInfoDto column)
        {
            var attrStr = "";
            if (column.IsPrimarykey)
            {
                attrStr = $"[SugarColumn(IsPrimaryKey = true,ColumnName = \"{column.ColumnName.ToLower()}\", {GetLength(column.Length)} IsNullable = {column.IsNullable.ToString().ToLower()}, ColumnDescription = \"{column.ColumnComment}\" {GetDefaultValue(column.DefaultValue)})]";
            }
            else if (column.IsIdentity)
            {
                attrStr = $@"[SugarColumn(IsIdentity = true,ColumnName = ""{column.ColumnName.ToLower()}"", {GetLength(column.Length)} IsNullable = {column.IsNullable.ToString().ToLower()}, ColumnDescription = ""{column.ColumnComment}""{GetDefaultValue(column.DefaultValue)})]";
            }
            if (string.IsNullOrEmpty(attrStr))
            {
                attrStr += $@"[SugarColumn(ColumnName = ""{column.ColumnName.ToLower()}"", {GetLength(column.Length)} IsNullable = {column.IsNullable.ToString().ToLower()}, ColumnDescription = ""{column.ColumnComment}"" {GetDefaultValue(column.DefaultValue)})]";
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


        private string GenerateDtoProperty(ColumnInfoDto column, bool isCreate = false, bool isOutput = false)
        {
            var validations = new List<string>();
            if (!column.IsNullable && !column.IsPrimarykey && !column.IsIdentity && isCreate)
            {
                validations.Add("[Required(ErrorMessage = \"" + column.ColumnComment + "不能为空\")]");
            }

            var lengthValidation = "";
            if (column.Length > 0 && (column.DataType?.Contains("char") == true || column.DataType?.Contains("text") == true))
            {
                if (!isOutput)
                {
                    lengthValidation = $"[StringLength({column.Length},ErrorMessage = \"{column.ColumnComment}最多{column.Length}个字符\")]";
                }
                else
                {
                    lengthValidation = "        ";
                }
            }

            var validationStr = string.Join(" ", validations);
            if (!string.IsNullOrEmpty(lengthValidation))
            {
                validationStr = string.IsNullOrEmpty(validationStr) ? lengthValidation : $"{validationStr}\n        {lengthValidation}";
            }

            var result = $"{GenGenerateColumnComment(column.ColumnName)}\n";
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


        public async Task<string>  GenerateEntityContentAsync(string tableName, string entityName, List<ColumnInfoDto> columns, string nameSpace, string TableComment)
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

        public async Task<string> GenerateDtoContentAsync(string entityName, List<ColumnInfoDto> columns, string nameSpace, string TableComment)
        {
            // 使用反射获取BaseEntity的所有公共属性名称
            var baseEntityProperties = GetBaseEntityPropertyNames();

            var filteredColumns = columns.Where(col =>
                !baseEntityProperties.Contains(col.ColumnName, StringComparer.OrdinalIgnoreCase) &&
                !baseEntityProperties.Contains(col.PropertyName, StringComparer.OrdinalIgnoreCase)).ToList();
            var primaryKeyColumns = columns.Where(x => x.IsPrimarykey).ToList();
            primaryKeyColumns.AddRange(filteredColumns);

            var createDtoContent = $@"using System.ComponentModel.DataAnnotations;

namespace {nameSpace ?? "Fastdotnet.Core.Models"}
{{
    /// <summary>
    ///新增传输模型
    /// </summary>
    public class Create{entityName}Dto
    {{
{string.Join("\n", filteredColumns.Where(col => !col.IsPrimarykey && !col.IsIdentity).Select(col => GenerateDtoProperty(col, true)))}
    }}

    /// <summary>
    ///修改传输模型
    /// </summary>
    public class Update{entityName}Dto
    {{
{string.Join("\n", primaryKeyColumns.Select(col => GenerateDtoProperty(col, false)))}
    }}

    /// <summary>
    ///输出传输模型
    /// </summary>
    public class {entityName}Dto
    {{
{string.Join("\n", columns.Where(x => x.ColumnName != "is_deleted" && x.ColumnName != "deleted_at").Select(col => GenerateDtoProperty(col, false, true)))}
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

namespace {nameSpace ?? "Fastdotnet.WebApi.Controllers"}
{{
    /// <summary>
    /// {entityName} 控制器
    /// </summary>
    [Route(""api/[controller]"")]
    public class {entityName}Controller : GenericDtoControllerBase<{entityName}, string, Create{entityName}Dto, Update{entityName}Dto, {entityName}Dto>
    {{
        private readonly IBaseService<{entityName}, string> _baseservice;

        public {entityName}Controller(
            //I{entityName}Service {entityName.ToLower()}Service,
            IBaseService<{entityName}, string> service,
            IMapper mapper) : base(service, mapper)
        {{
            _baseservice = service;
        }}
    }}
}}";
        }


        public async Task<string> GenerateFrontendVueContentAsync(string entityName, List<ColumnInfoDto> columns, string busName, string pagePath, string TableComment, List<FdCodeGenConfig> configcolumns)
        {
            // 使用反射获取BaseEntity的所有公共属性名称
            var baseEntityProperties = GetBaseEntityPropertyNames();

            //var filteredColumns = columns.Where(col =>
            //    !baseEntityProperties.Contains(col.ColumnName, StringComparer.OrdinalIgnoreCase) &&
            //    !baseEntityProperties.Contains(col.PropertyName, StringComparer.OrdinalIgnoreCase)).ToList();
            //var configcolumnsisshow = configcolumns.Where(x=>x.WhetherTable=="是");
            var WhetherQuery = configcolumns.Where(x => x.WhetherQuery == "是" && x.QueryType == "BETWEEN");
            return $@"<template>
	<div class=""{entityName.ToLower()}-container"">
		<el-card shadow=""hover"" :body-style=""{{ padding: 2 }}"">
			<el-form :model=""state.queryParams"" ref=""queryForm"" :inline=""true"">
                <div v-show=""state.searchCollapsed"" >
                    {string.Join("\n\t", configcolumns.Where(x => x.WhetherQuery == "是").Select((col, idx) =>
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
				{string.Join("\n\t\t\t\t", configcolumns.Where(x => x.WhetherTable == "是").Select((col, idx) =>
                    $"				<el-table-column prop=\"{col.PropertyName}\" label=\"{col.ShowColumnName ?? col.PropertyName}\" align=\"center\" show-overflow-tooltip />"
                ))}
				<el-table-column label=""操作"" width=""180"" fixed=""right"" align=""center"">
					<template #default=""scope"">
						<el-button icon=""ele-Edit"" size=""small"" text type=""primary"" @click=""openEditDialog(scope.row)"">修改</el-button>
						<el-button icon=""ele-Delete"" size=""small"" text type=""danger"" @click=""handleDelete(scope.row)"">删除</el-button>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination
				v-model:currentPage=""state.tableParams.page""
				v-model:page-size=""state.tableParams.pageSize""
				:total=""state.tableParams.total""
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
				{string.Join("\n\t\t\t\t", configcolumns.Where(x => x.WhetherAddUpdate == "是").Select(col =>
                    $"				<el-col :xs=\"24\" :sm=\"12\" :md=\"12\" :lg=\"12\" :xl=\"12\" class=\"mb20\">\n					<el-form-item label=\"{col.ShowColumnName ?? col.PropertyName}\" prop=\"{col.PropertyName}\">\n						<el-input v-model=\"state.formData.{col.PropertyName}\" placeholder=\"请输入{col.ShowColumnName ?? col.PropertyName}\" {getFrontendmMaxLenghtTemp(col, columns)} clearable />\n					</el-form-item>\n				</el-col>"
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
import {{buildMixedQuery}} from '/@/utils/queryBuilder';
import type {{{entityName}}} from '/@/api/fd-system-api/typings';
import dayjs from 'dayjs'; // 引入日期处理库
const queryForm = ref();
const formRef = ref();

const state = reactive({{
	loading: false,
    searchCollapsed: true, 
	tableData: {{
		data: [],
		total: 0,
		loading: false,
		param: {{
			pageNum: 1,
			pageSize: 10,
		}},
	}},
	queryParams: {{
	{string.Join("\n\t", configcolumns.Where(x => x.WhetherQuery == "是").Select((col, idx) => $@"{col.PropertyName}:null,"))}
	{string.Join("\n\t", configcolumns.Where(x => x.WhetherQuery == "是"&&x.QueryType == "BETWEEN").Select((col, idx) => $@"{col.PropertyName}_1:null,"))}
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
	formData: {{
	{string.Join("\n\t", configcolumns.Where(x => x.WhetherAddUpdate == "是").Select((col, idx) => $@"{col.PropertyName}:'',"))}
}}
}});
const toggleSearchCollapse = () => {{
	state.searchCollapsed = !state.searchCollapsed;
}};
// 获取列表
const getList = async () => {{
	state.loading = true;
	//构建查询条件
    const queryConfig = 
            {{
    {GetFrontendCondition(configcolumns)}
    }}
	const searchBody: APIModel.PageQueryByConditionDto = {{
		PageIndex: state.tableData.param.pageNum,
		PageSize: state.tableData.param.pageSize
	}};
	const queryResult = buildMixedQuery(queryConfig);
	if (queryResult.dynamicQuery) {{
		searchBody.DynamicQuery = queryResult.dynamicQuery;
		searchBody.QueryParameters = queryResult.queryParameters;
	}}
    // 调试日志
    console.log('Search request body:', searchBody);
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
			if (state.formData.{configcolumns.Where(w => w.ColumnKey == true).FirstOrDefault().PropertyName ?? configcolumns.FirstOrDefault().PropertyName}) {{
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
<style scoped lang=""scss"">
.el-form--inline .el-form-item {{
	margin-right: 12px !important; // 稍微紧凑一点
	margin-bottom: 8px !important;
}}
.{entityName.ToLower()}-container .el-card:first-child .el-form .el-form-item:last-of-type {{
    margin-bottom: 0 !important;
}}
</style>


"


;
        }

        /// <summary>
        /// 获取前端最大输入值模板
        /// </summary>
        /// <returns></returns>
        private string getFrontendmMaxLenghtTemp(FdCodeGenConfig fdCodeGenConfig, List<ColumnInfoDto> columns)
        {
            var columnInfoDto = columns.FirstOrDefault(w => w.ColumnName == fdCodeGenConfig.ColumnName);
            if (columnInfoDto.Length > 0)
            {
                return $@"maxlength=""{columnInfoDto.Length}"" show-word-limit";
            }
            return string.Empty;
        }


        private string GetFrontendCondition(List<FdCodeGenConfig> fdCodeGenConfig)
        {
            var ranges = new Dictionary<string, Dictionary<string, object>>();
            var jsObject = new global::System.Text.StringBuilder();
            jsObject.Append("ranges:{");

            foreach (var item in fdCodeGenConfig.Where(x => x.WhetherQuery == "是" && x.QueryType == "BETWEEN"))
            {
                jsObject.Append("\n" + $@"{item.PropertyName}:" + "{\n\tfrom:" + $@"state.queryParams.{item.PropertyName}," + "\n\tto:" + $@"state.queryParams.{item.PropertyName}_1" + "},\n");

            }
            jsObject.Append("}");
            var sb = new global::System.Text.StringBuilder();
            sb.Append("customs:[\n");
            foreach (var item in fdCodeGenConfig.Where(x => x.WhetherQuery == "是" && x.QueryType != "BETWEEN"))
            {
                sb.Append($@"{{field:'{item.PropertyName}',operator:'{item.QueryType}',value:state.queryParams.{item.PropertyName},}}," + "\n");
            }
            sb.Append("]");
            return sb.ToString() + ",\n" + jsObject.ToString();
        }

        private string getFrontendQueryTemp(FdCodeGenConfig fdCodeGenConfig)
        {
            if (fdCodeGenConfig.WhetherQuery == "是" && fdCodeGenConfig.QueryType == "BETWEEN")
            {
                return $@"<el-form-item label=""{fdCodeGenConfig.ShowColumnName}"" prop=""{fdCodeGenConfig.PropertyName}"">
                <div style=""display: flex; gap: 8px;"">
                    <el-input placeholder=""请输入起始{fdCodeGenConfig.ShowColumnName}"" clearable v-model=""state.queryParams.{fdCodeGenConfig.PropertyName}"" style=""flex: 1;"" />
                    <span style=""align-self: center;"">-</span>
                    <el-input placeholder=""请输入结束{fdCodeGenConfig.ShowColumnName}"" clearable v-model=""state.queryParams.{fdCodeGenConfig.PropertyName}_1"" style=""flex: 1;"" />
                </div>
            </el-form-item>
            <el-form-item prop=""{fdCodeGenConfig.PropertyName}_1"" style=""display:none;""></el-form-item>";
            }

            else
            {
                return $@"<el-form-item label=""{fdCodeGenConfig.ShowColumnName}"" prop=""{fdCodeGenConfig.PropertyName}"">
					<el-input placeholder=""请输入{fdCodeGenConfig.ShowColumnName}"" clearable  v-model=""state.queryParams.{fdCodeGenConfig.PropertyName}"" style=""width: 150px""/>
				</el-form-item>";
            }

        }
    }
}