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
        /// 执行代码生成
        /// </summary>
        /// <param name="input">代码生成输入</param>
        /// <returns></returns>
        Task<string> GenerateCodeAsync(CodeGenInput input);
    }
}