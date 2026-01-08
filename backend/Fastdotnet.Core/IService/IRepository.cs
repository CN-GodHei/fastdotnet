using Fastdotnet.Core.Dtos;
using Fastdotnet.Core.Dtos.Base;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Fastdotnet.Core.IService
{
    /// <summary>
    /// 通用仓储接口
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <typeparam name="TKey">主键类型</typeparam>
    public interface IRepository<T, TKey> where T : BaseEntity, new()
    {
        #region 查询操作

        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>实体对象</returns>
        Task<T> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <returns>实体列表</returns>
        Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 根据条件查询实体列表
        /// </summary>
        /// <param name="whereExpression">查询条件表达式</param>
        /// <returns>实体列表</returns>
        Task<List<T>> GetListAsync(Expression<Func<T, bool>> whereExpression, CancellationToken cancellationToken = default);

        // 👇 新增：支持 Select 投影
        Task<List<TResult>> GetListAsync<TResult>(
            Expression<Func<T, bool>> whereExpression,
            Expression<Func<T, TResult>> selectExpression,
            CancellationToken ct = default);
        /// <summary>
        /// 根据条件查询第一个实体
        /// </summary>
        /// <param name="whereExpression">查询条件表达式</param>
        /// <returns>实体对象</returns>
        Task<T> GetFirstAsync(Expression<Func<T, bool>> whereExpression, CancellationToken cancellationToken = default);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="orderByExpression">排序表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns>分页结果</returns>
        Task<PageResult<T>> GetPageAsync(
            int pageIndex,
            int pageSize,
            Expression<Func<T, object>> orderByExpression = null,
            OrderByType orderByType = OrderByType.Asc, CancellationToken cancellationToken = default);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="whereExpression">查询条件表达式</param>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="orderByExpression">排序表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns>分页结果</returns>
        Task<PageResult<T>> GetPageAsync(
            Expression<Func<T, bool>> whereExpression,
            int pageIndex,
            int pageSize,
            Expression<Func<T, object>> orderByExpression = null,
            OrderByType orderByType = OrderByType.Asc, CancellationToken cancellationToken = default);

        /// <summary>
        /// 判断是否存在满足条件的实体
        /// </summary>
        /// <param name="whereExpression">查询条件表达式</param>
        /// <returns>是否存在</returns>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> whereExpression);

        #endregion

        #region 插入操作

        /// <summary>
        /// 插入实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>插入后的实体</returns>
        Task<T> InsertAsync(T entity);

        /// <summary>
        /// 批量插入实体
        /// </summary>
        /// <param name="entities">实体列表</param>
        /// <returns>插入成功的数量</returns>
        Task<int> InsertRangeAsync(List<T> entities);

        #endregion

        #region 更新操作

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>更新后的实体</returns>
        Task<T> UpdateAsync(T entity);

        /// <summary>
        /// 根据主键更新指定字段
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="columns">要更新的字段</param>
        /// <returns>是否更新成功</returns>
        Task<bool> UpdateAsync(TKey id, Dictionary<string, object> columns);

        /// <summary>
        /// 批量更新实体
        /// </summary>
        /// <param name="entities">实体列表</param>
        /// <returns>更新成功的数量</returns>
        Task<int> UpdateRangeAsync(List<T> entities);

        /// <summary>
        /// 根据条件批量更新实体
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="columns">要更新的字段</param>
        /// <returns>更新成功的数量</returns>
        Task<int> UpdateRangeAsync(Expression<Func<T, bool>> whereExpression, Dictionary<string, object> columns);

        #endregion

        #region 删除操作

        /// <summary>
        /// 根据主键删除实体（软删除）
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>是否删除成功</returns>
        Task<bool> DeleteAsync(TKey id);

        /// <summary>
        /// 根据条件批量删除实体（软删除）
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <returns>删除成功的数量</returns>
        Task<int> DeleteAsync(Expression<Func<T, bool>> whereExpression);

        #endregion

        #region 回收站操作

        /// <summary>
        /// 获取回收站数据（已删除但未被物理删除的数据）
        /// </summary>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="orderByExpression">排序表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns>分页结果</returns>
        Task<PageResult<T>> GetRecycleBinAsync(
            int pageIndex,
            int pageSize,
            Expression<Func<T, object>> orderByExpression = null,
            OrderByType orderByType = OrderByType.Desc, CancellationToken cancellationToken = default);

        /// <summary>
        /// 根据条件查询回收站数据
        /// </summary>
        /// <param name="whereExpression">查询条件表达式</param>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="orderByExpression">排序表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns>分页结果</returns>
        Task<PageResult<T>> GetRecycleBinAsync(
            Expression<Func<T, bool>> whereExpression,
            int pageIndex,
            int pageSize,
            Expression<Func<T, object>> orderByExpression = null,
            OrderByType orderByType = OrderByType.Desc, CancellationToken cancellationToken = default);

        /// <summary>
        /// 恢复回收站中的实体
        /// </summary>
        /// <param name="id">实体主键</param>
        /// <returns>是否恢复成功</returns>
        Task<bool> RestoreAsync(TKey id);

        /// <summary>
        /// 批量恢复回收站中的实体
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <returns>恢复成功的数量</returns>
        Task<int> RestoreAsync(Expression<Func<T, bool>> whereExpression);

        /// <summary>
        /// 永久删除回收站中的实体
        /// </summary>
        /// <param name="id">实体主键</param>
        /// <returns>是否删除成功</returns>
        Task<bool> PermanentDeleteAsync(TKey id);

        /// <summary>
        /// 根据条件永久删除回收站中的实体
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <returns>删除成功的数量</returns>
        Task<int> PermanentDeleteAsync(Expression<Func<T, bool>> whereExpression);

        #endregion
        Task<T> InsertOrUpdateAsync(T entity);
        Task<T> AddAndUpdateItByPrimaryKeysAsync(T entity);
    }

    /// <summary>
    /// 默认主键为string的仓储接口
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    public interface IRepository<T> : IRepository<T, string> where T : BaseEntity, new()
    {
    }
}