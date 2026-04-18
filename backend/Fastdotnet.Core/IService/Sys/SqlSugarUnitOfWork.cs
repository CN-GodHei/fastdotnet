
namespace Fastdotnet.Core.IService.Sys
{
    // 2. SqlSugar 实现
    public class SqlSugarUnitOfWork : IUnitOfWork, IStorageContext
    {
        private readonly ISqlSugarClient _db;

        public SqlSugarUnitOfWork(ISqlSugarClient db) => _db = db;

        public Task BeginTransactionAsync(CancellationToken ct = default)
            => _db.Ado.BeginTranAsync();

        public Task CommitAsync(CancellationToken ct = default)
            => _db.Ado.CommitTranAsync();

        public Task RollbackAsync(CancellationToken ct = default)
            => _db.Ado.RollbackTranAsync();

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
            
            // 【优化】优先通过 SugarColumn 特性查找主键，支持自定义主键名称
            var idProperty = type.GetProperties()
                .FirstOrDefault(p => p.GetCustomAttribute<SugarColumn>()?.IsPrimaryKey == true);

            // 兼容旧逻辑：如果没有标记特性，则尝试查找常见的 Id 命名
            if (idProperty == null)
            {
                idProperty = type.GetProperty("Id") ?? 
                             type.GetProperty("ID") ?? 
                             type.GetProperty("id");
            }
            
            if (idProperty != null)
            {
                return idProperty.GetValue(entity);
            }
            
            throw new InvalidOperationException($"Entity {typeof(T).Name} does not have a primary key property marked with [SugarColumn(IsPrimaryKey = true)] or named 'Id'");
        }


    }
}