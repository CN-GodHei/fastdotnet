

namespace Fastdotnet.Core.IService.Sys
{
    /// <summary>
    /// 无约束通用仓储接口
    /// 适用于任意 class（无需继承 BaseEntity 或实现 IEntity）
    /// 不自动处理软删除、创建时间等业务字段
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <typeparam name="TKey">主键类型</typeparam>
    public interface IRawRepository<T, TKey> where T : class, new()
    {
        #region 查询

        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>实体对象，不存在则返回 null</returns>
        Task<T?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>实体列表</returns>
        Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 根据条件查询实体列表
        /// </summary>
        /// <param name="whereExpression">查询条件表达式</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>实体列表</returns>
        Task<List<T>> GetListAsync(Expression<Func<T, bool>> whereExpression, CancellationToken cancellationToken = default);

        /// <summary>
        /// 根据条件查询第一个匹配的实体
        /// </summary>
        /// <param name="whereExpression">查询条件表达式</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>实体对象，不存在则返回 null</returns>
        Task<T?> GetFirstAsync(Expression<Func<T, bool>> whereExpression, CancellationToken cancellationToken = default);

        /// <summary>
        /// 判断是否存在满足条件的实体
        /// </summary>
        /// <param name="whereExpression">查询条件表达式</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>是否存在</returns>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> whereExpression, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取实体总数
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>记录总数</returns>
        Task<int> CountAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 根据条件获取实体数量
        /// </summary>
        /// <param name="whereExpression">查询条件表达式</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>满足条件的记录数</returns>
        Task<int> CountAsync(Expression<Func<T, bool>> whereExpression, CancellationToken cancellationToken = default);

        #endregion

        #region 插入

        /// <summary>
        /// 插入单个实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>插入后的实体（可能包含数据库生成的值，如自增ID）</returns>
        Task<T> InsertAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// 批量插入实体
        /// </summary>
        /// <param name="entities">实体列表</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>插入成功的记录数</returns>
        Task<int> InsertRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

        #endregion

        #region 更新

        /// <summary>
        /// 更新整个实体（全字段更新）
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>是否更新成功</returns>
        Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// 批量更新实体（全字段更新）
        /// </summary>
        /// <param name="entities">实体列表</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>更新成功的记录数</returns>
        Task<int> UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// 根据条件批量更新指定字段（字典形式）
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="columns">字段名与新值的映射</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>更新成功的记录数</returns>
        Task<int> UpdateRangeAsync(
            Expression<Func<T, bool>> whereExpression,
            Dictionary<string, object> columns,
            CancellationToken cancellationToken = default);

        #endregion

        #region 删除

        /// <summary>
        /// 根据主键删除实体（物理删除）
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>是否删除成功</returns>
        Task<bool> DeleteAsync(TKey id, CancellationToken cancellationToken = default);

        /// <summary>
        /// 根据实体对象删除（物理删除）
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>是否删除成功</returns>
        Task<bool> DeleteAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// 根据条件批量删除实体（物理删除）
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>删除成功的记录数</returns>
        Task<int> DeleteRangeAsync(Expression<Func<T, bool>> whereExpression, CancellationToken cancellationToken = default);

        Task<T> InsertOrUpdateAsync(T entity);
        Task<T> AddAndUpdateItByPrimaryKeysAsync(T entity);
        #endregion

        /// <summary>
        /// 无约束通用仓储（主键默认为 string）
        /// </summary>

    }
    public interface IRawRepository<T> : IRawRepository<T, string> where T : class, new() { }

}