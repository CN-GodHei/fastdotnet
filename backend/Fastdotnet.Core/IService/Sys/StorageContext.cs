
namespace Fastdotnet.Core.IService.Sys
{
    public class StorageContext : IStorageContext
    {
        private readonly ISqlSugarClient _db;

        public StorageContext(ISqlSugarClient db)
        {
            _db = db;
        }

        public async Task<T?> QuerySingleOrDefaultAsync<T>(
            string sql,
            object? parameters = null,
            CancellationToken ct = default) where T : class, new()
        {
            var result = await _db.Ado.SqlQueryAsync<T>(sql, parameters);
            return result.FirstOrDefault();
        }

        public async Task<int> ExecuteNonQueryAsync(
            string sql,
            object? parameters = null,
            CancellationToken ct = default)
        {
            return await _db.Ado.ExecuteCommandAsync(sql, parameters);
        }

        public async Task SaveEntityAsync<T>(
            T entity,
            CancellationToken ct = default) where T : class, new()
        {
            // 优化：使用 SqlSugar 的 Saveable 方法，自动根据主键判断插入或更新
            // 这种方式比反射获取 ID 更高效，且代码更简洁
            await _db.Saveable(entity).ExecuteCommandAsync();
        }
    }
}