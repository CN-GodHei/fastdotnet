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
            var sql = $"SELECT * FROM {TableName} WHERE FdAppUserId = @userId";
            return await context.QuerySingleOrDefaultAsync<TData>(sql, new { userId }, ct);
        }

        /// <summary>
        /// 设置用户ID到扩展数据中（由子类实现）
        /// </summary>
        /// <param name="data">扩展数据</param>
        /// <param name="userId">用户ID</param>
        protected abstract void SetUserId(TData data, string userId);
    }
}
