using System.Reflection;
using SqlSugar;

namespace Fastdotnet.Core.Extensibility.Users
{
    /// <summary>
    /// 用户扩展处理器基类，提供通用的保存和加载逻辑
    /// </summary>
    /// <typeparam name="TData">扩展数据类型</typeparam>
    public abstract class FdAppUserExtensionHandlerBase<TData> : IFdAppUserExtensionHandler<TData> 
        where TData : class, new()
    {
        protected readonly IStorageContext StorageContext;
        protected readonly string TableName;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="storageContext">存储上下文</param>
        /// <param name="tableName">数据库表名</param>
        protected FdAppUserExtensionHandlerBase(IStorageContext storageContext, string tableName)
        {
            StorageContext = storageContext;
            TableName = tableName;
        }

        /// <summary>
        /// 保存用户扩展数据
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="data">扩展数据</param>
        /// <param name="context">存储上下文</param>
        /// <param name="ct">取消令牌</param>
        public virtual async Task SaveAsync(string userId, TData data, IStorageContext context, CancellationToken ct = default)
        {
            // 设置用户ID（由子类实现具体逻辑）
            SetUserId(data, userId);
            
            // 保存数据
            await context.SaveEntityAsync(data, ct);
        }

        /// <summary>
        /// 加载用户扩展数据
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="context">存储上下文</param>
        /// <param name="ct">取消令牌</param>
        /// <returns>扩展数据，如果不存在则返回 null</returns>
        public virtual async Task<TData?> LoadAsync(string userId, IStorageContext context, CancellationToken ct = default)
        {
            // 动态获取实体的主键字段名
            var primaryKeyPropertyName = GetPrimaryKeyPropertyName();
            var sql = $"SELECT * FROM {TableName} WHERE {primaryKeyPropertyName} = @userId";
            return await context.QuerySingleOrDefaultAsync<TData>(sql, new { userId }, ct);
        }

        /// <summary>
        /// 设置用户ID到扩展数据中（由子类实现）
        /// </summary>
        /// <param name="data">扩展数据</param>
        /// <param name="userId">用户ID</param>
        protected abstract void SetUserId(TData data, string userId);

        /// <summary>
        /// 获取实体的主键字段名
        /// </summary>
        /// <returns>主键字段名</returns>
        protected string GetPrimaryKeyPropertyName()
        {
            var type = typeof(TData);
            
            // 优先通过 SugarColumn 特性查找主键，支持自定义主键名称
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
                // 检查是否有 SugarColumn 特性并指定了 ColumnName
                var sugarColumnAttr = idProperty.GetCustomAttribute<SugarColumn>();
                if (sugarColumnAttr != null && !string.IsNullOrEmpty(sugarColumnAttr.ColumnName))
                {
                    return sugarColumnAttr.ColumnName;
                }
                return idProperty.Name;
            }
            
            throw new InvalidOperationException($"Entity {typeof(TData).Name} does not have a primary key property marked with [SugarColumn(IsPrimaryKey = true)] or named 'Id'");
        }
    }
}
