using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fastdotnet.Plugin.Contracts
{
    public interface IStorageContext
    {
        /// <summary>
        /// 查询单个实体（支持参数化 SQL）
        /// </summary>
        Task<T?> QuerySingleOrDefaultAsync<T>(
            string sql,
            object? parameters = null,
            CancellationToken ct = default) where T : class, new();

        /// <summary>
        /// 执行非查询命令（INSERT/UPDATE/DELETE）
        /// </summary>
        Task<int> ExecuteNonQueryAsync(
            string sql,
            object? parameters = null,
            CancellationToken ct = default);

        /// <summary>
        /// 保存实体（插入或更新，由实现决定）
        /// </summary>
        Task SaveEntityAsync<T>(
            T entity,
            CancellationToken ct = default) where T : class, new();
    }
}
