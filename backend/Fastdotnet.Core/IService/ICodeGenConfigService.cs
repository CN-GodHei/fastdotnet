using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Models.System;

namespace Fastdotnet.Core.IService
{
    public interface ICodeGenConfigService
    {
        /// <summary>
        /// 获取数据库表列表
        /// </summary>
        /// <returns></returns>
        Task<List<TableInfoDto>> GetTableListAsync();

        /// <summary>
        /// 获取表列信息列表
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        Task<List<ColumnInfoDto>> GetTableColumnListAsync(string tableName);

        /// <summary>
        /// 根据表名获取实体名
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        string GetEntityNameByTableName(string tableName);

        
        /// <summary>
        /// 生成实体类代码
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="entityName">实体名</param>
        /// <param name="columns">列信息列表</param>
        /// <param name="nameSpace">命名空间</param>
        /// <returns></returns>
        Task<string> GenerateEntityContentAsync(string tableName, string entityName, List<FdCodeGenConfig> columns, string nameSpace,string TableComment);
        
        /// <summary>
        /// 生成DTO代码
        /// </summary>
        /// <param name="entityName">实体名</param>
        /// <param name="columns">列信息列表</param>
        /// <param name="nameSpace">命名空间</param>
        /// <returns></returns>
        Task<string> GenerateDtoContentAsync(string entityName, List<FdCodeGenConfig> columns, string nameSpace, string TableComment);
        
        /// <summary>
        /// 生成服务实现代码
        /// </summary>
        /// <param name="entityName">实体名</param>
        /// <param name="nameSpace">命名空间</param>
        /// <returns></returns>
        Task<string> GenerateServiceImplementationContentAsync(string entityName, string nameSpace, string TableComment);
        
        /// <summary>
        /// 生成控制器代码
        /// </summary>
        /// <param name="entityName">实体名</param>
        /// <param name="nameSpace">命名空间</param>
        /// <returns></returns>
        Task<string> GenerateControllerContentAsync(string entityName, string nameSpace, string TableComment);
        
        /// <summary>
        /// 生成前端页面代码
        /// </summary>
        /// <param name="entityName">实体名</param>
        /// <param name="columns">列信息列表</param>
        /// <param name="tableName">表名</param>
        /// <param name="pagePath">页面路径</param>
        /// <returns></returns>
        Task<string> GenerateFrontendVueContentAsync(string entityName, string tableName, string pagePath, string TableComment, List<FdCodeGenConfig> configcolumns);

        string GetEffectTypeByColumnName(string columnName, string dataType);

    }
}