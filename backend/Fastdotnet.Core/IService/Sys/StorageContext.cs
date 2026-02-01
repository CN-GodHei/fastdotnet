
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
            // 检查实体是否已存在，如果存在则更新，否则插入
            var table = _db.GetSimpleClient<T>();
            var existing = await table.GetByIdAsync(GetEntityId(entity));
            
            if (existing != null)
            {
                await table.UpdateAsync(entity); // 移除返回值
            }
            else
            {
                await table.InsertReturnIdentityAsync(entity); // 移除返回值
            }
        }
        
        private object GetEntityId<T>(T entity) where T : class, new()
        {
            var type = typeof(T);
            var idProperty = type.GetProperty("Id") ?? 
                           type.GetProperty("ID") ?? 
                           type.GetProperty("id");
            
            if (idProperty != null)
            {
                return idProperty.GetValue(entity);
            }
            
            throw new InvalidOperationException($"Entity {typeof(T).Name} does not have an Id property");
        }
    }
}