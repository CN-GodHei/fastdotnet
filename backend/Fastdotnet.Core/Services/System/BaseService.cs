using Fastdotnet.Core.IService;
using Fastdotnet.Core.Models;
using Fastdotnet.Core.Models.Base;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Fastdotnet.Core.Services.System
{
    /// <summary>
    /// 基础服务实现类
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TKey">主键类型</typeparam>
    public class BaseService<TEntity, TKey> : IBaseService<TEntity, TKey> where TEntity : BaseEntity, new()
    {
        protected readonly IRepository<TEntity, TKey> _repository;

        public BaseService(IRepository<TEntity, TKey> repository)
        {
            _repository = repository;
        }

        #region 查询操作

        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>实体对象</returns>
        public virtual async Task<TEntity> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return await _repository.GetByIdAsync(id, cancellationToken);
        }

        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <returns>实体列表</returns>
        public virtual async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _repository.GetAllAsync(cancellationToken);
        }

        /// <summary>
        /// 根据条件查询实体列表
        /// </summary>
        /// <param name="whereExpression">查询条件表达式</param>
        /// <returns>实体列表</returns>
        public virtual async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> whereExpression, CancellationToken cancellationToken = default)
        {
            return await _repository.GetListAsync(whereExpression, cancellationToken);
        }

        /// <summary>
        /// 根据条件查询第一个实体
        /// </summary>
        /// <param name="whereExpression">查询条件表达式</param>
        /// <returns>实体对象</returns>
        public virtual async Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> whereExpression, CancellationToken cancellationToken = default)
        {
            return await _repository.GetFirstAsync(whereExpression, cancellationToken);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="orderByExpression">排序表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns>分页结果</returns>
        public virtual async Task<PageResult<TEntity>> GetPageAsync(
            int pageIndex,
            int pageSize,
            Expression<Func<TEntity, object>> orderByExpression = null,
            OrderByType orderByType = OrderByType.Asc, CancellationToken cancellationToken = default)
        {
            return await _repository.GetPageAsync(pageIndex, pageSize, orderByExpression, orderByType, cancellationToken);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="whereExpression">查询条件表达式</param>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="orderByExpression">排序表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns>分页结果</returns>
        public virtual async Task<PageResult<TEntity>> GetPageAsync(
            Expression<Func<TEntity, bool>> whereExpression,
            int pageIndex,
            int pageSize,
            Expression<Func<TEntity, object>> orderByExpression = null,
            OrderByType orderByType = OrderByType.Asc, CancellationToken cancellationToken = default)
        {
            return await _repository.GetPageAsync(whereExpression, pageIndex, pageSize, orderByExpression, orderByType, cancellationToken);
        }

        /// <summary>
        /// 判断是否存在满足条件的实体
        /// </summary>
        /// <param name="whereExpression">查询条件表达式</param>
        /// <returns>是否存在</returns>
        public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> whereExpression, CancellationToken cancellationToken = default)
        {
            return await _repository.ExistsAsync(whereExpression);
        }

        #endregion

        #region 插入操作

        /// <summary>
        /// 插入实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>插入后的实体</returns>
        public virtual async Task<TEntity> InsertAsync(TEntity entity)
        {
            return await _repository.InsertAsync(entity);
        }

        /// <summary>
        /// 批量插入实体
        /// </summary>
        /// <param name="entities">实体列表</param>
        /// <returns>插入成功的数量</returns>
        public virtual async Task<int> InsertRangeAsync(List<TEntity> entities)
        {
            return await _repository.InsertRangeAsync(entities);
        }

        #endregion

        #region 更新操作

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>更新后的实体</returns>
        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            return await _repository.UpdateAsync(entity);
        }

        /// <summary>
        /// 根据主键更新指定字段
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="columns">要更新的字段</param>
        /// <returns>是否更新成功</returns>
        public virtual async Task<bool> UpdateAsync(TKey id, Dictionary<string, object> columns)
        {
            return await _repository.UpdateAsync(id, columns);
        }

        /// <summary>
        /// 批量更新实体
        /// </summary>
        /// <param name="entities">实体列表</param>
        /// <returns>更新成功的数量</returns>
        public virtual async Task<int> UpdateRangeAsync(List<TEntity> entities)
        {
            return await _repository.UpdateRangeAsync(entities);
        }

        /// <summary>
        /// 根据条件批量更新实体
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="columns">要更新的字段</param>
        /// <returns>更新成功的数量</returns>
        public virtual async Task<int> UpdateRangeAsync(Expression<Func<TEntity, bool>> whereExpression, Dictionary<string, object> columns)
        {
            return await _repository.UpdateRangeAsync(whereExpression, columns);
        }

        #endregion

        #region 删除操作

        /// <summary>
        /// 根据主键删除实体（软删除）
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>是否删除成功</returns>
        public virtual async Task<bool> DeleteAsync(TKey id)
        {
            return await _repository.DeleteAsync(id);
        }

        /// <summary>
        /// 根据条件批量删除实体（软删除）
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <returns>删除成功的数量</returns>
        public virtual async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> whereExpression)
        {
            return await _repository.DeleteAsync(whereExpression);
        }

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
        public virtual async Task<PageResult<TEntity>> GetRecycleBinAsync(
            int pageIndex,
            int pageSize,
            Expression<Func<TEntity, object>> orderByExpression = null,
            OrderByType orderByType = OrderByType.Desc, CancellationToken cancellationToken = default)
        {
            return await _repository.GetRecycleBinAsync(pageIndex, pageSize, orderByExpression, orderByType, cancellationToken);
        }

        /// <summary>
        /// 根据条件查询回收站数据
        /// </summary>
        /// <param name="whereExpression">查询条件表达式</param>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="orderByExpression">排序表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns>分页结果</returns>
        public virtual async Task<PageResult<TEntity>> GetRecycleBinAsync(
            Expression<Func<TEntity, bool>> whereExpression,
            int pageIndex,
            int pageSize,
            Expression<Func<TEntity, object>> orderByExpression = null,
            OrderByType orderByType = OrderByType.Desc, CancellationToken cancellationToken = default)
        {
            return await _repository.GetRecycleBinAsync(whereExpression, pageIndex, pageSize, orderByExpression, orderByType, cancellationToken);
        }

        /// <summary>
        /// 恢复回收站中的实体
        /// </summary>
        /// <param name="id">实体主键</param>
        /// <returns>是否恢复成功</returns>
        public virtual async Task<bool> RestoreAsync(TKey id)
        {
            return await _repository.RestoreAsync(id);
        }

        /// <summary>
        /// 批量恢复回收站中的实体
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <returns>恢复成功的数量</returns>
        public virtual async Task<int> RestoreAsync(Expression<Func<TEntity, bool>> whereExpression)
        {
            return await _repository.RestoreAsync(whereExpression);
        }

        /// <summary>
        /// 永久删除回收站中的实体
        /// </summary>
        /// <param name="id">实体主键</param>
        /// <returns>是否删除成功</returns>
        public virtual async Task<bool> PermanentDeleteAsync(TKey id)
        {
            return await _repository.PermanentDeleteAsync(id);
        }

        /// <summary>
        /// 根据条件永久删除回收站中的实体
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <returns>删除成功的数量</returns>
        public virtual async Task<int> PermanentDeleteAsync(Expression<Func<TEntity, bool>> whereExpression)
        {
            return await _repository.PermanentDeleteAsync(whereExpression);
        }

        #endregion
    }

    /// <summary>
    /// 默认主键为string的基础服务实现类
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    public class BaseService<TEntity> : BaseService<TEntity, string>, IBaseService<TEntity> where TEntity : BaseEntity, new()
    {
        public BaseService(IRepository<TEntity, string> repository) : base(repository)
        {
        }
    }
}