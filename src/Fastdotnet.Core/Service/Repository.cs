using Fastdotnet.Core.IService;
using Fastdotnet.Core.Models;
using Fastdotnet.Core.Models.Base;
using Fastdotnet.Core.Models.Interfaces;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Fastdotnet.Core.Service
{
    /// <summary>
    /// 通用仓储实现
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <typeparam name="TKey">主键类型</typeparam>
    public class Repository<T, TKey> : IRepository<T, TKey> where T : BaseEntity, new()
    {
        protected readonly ISqlSugarClient _db;

        public Repository(ISqlSugarClient db)
        {
            _db = db;
        }

        #region 查询操作

        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>实体对象</returns>
        public virtual async Task<T> GetByIdAsync(TKey id)
        {
            return await _db.Queryable<T>().InSingleAsync(id);
        }

        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <returns>实体列表</returns>
        public virtual async Task<List<T>> GetAllAsync()
        {
            return await _db.Queryable<T>().ToListAsync();
        }

        /// <summary>
        /// 根据条件查询实体列表
        /// </summary>
        /// <param name="whereExpression">查询条件表达式</param>
        /// <returns>实体列表</returns>
        public virtual async Task<List<T>> GetListAsync(Expression<Func<T, bool>> whereExpression)
        {
            return await _db.Queryable<T>().WhereIF(whereExpression != null, whereExpression).ToListAsync();
        }

        /// <summary>
        /// 根据条件查询第一个实体
        /// </summary>
        /// <param name="whereExpression">查询条件表达式</param>
        /// <returns>实体对象</returns>
        public virtual async Task<T> GetFirstAsync(Expression<Func<T, bool>> whereExpression)
        {
            return await _db.Queryable<T>().WhereIF(whereExpression != null, whereExpression).FirstAsync();
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="whereExpression">查询条件表达式</param>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="orderByExpression">排序表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns>分页结果</returns>
        public virtual async Task<PageResult<T>> GetPageAsync(
            Expression<Func<T, bool>> whereExpression,
            int pageIndex,
            int pageSize,
            Expression<Func<T, object>> orderByExpression = null,
            OrderByType orderByType = OrderByType.Asc)
        {
            RefAsync<int> totalCount = 0;
            var query = _db.Queryable<T>().WhereIF(whereExpression != null, whereExpression);

            if (orderByExpression != null)
            {
                query = query.OrderBy(orderByExpression, orderByType);
            }
            var list = await query.ToPageListAsync(pageIndex, pageSize, totalCount);
            return new PageResult<T>
            {
                Items = list,
                PageInfo = new PageInfo()
                {
                    Page = pageIndex,
                    PageSize = pageSize,
                    Total = totalCount.Value
                }
            };
        }


        /// <summary>
        /// 判断是否存在满足条件的实体
        /// </summary>
        /// <param name="whereExpression">查询条件表达式</param>
        /// <returns>是否存在</returns>
        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> whereExpression)
        {
            return await _db.Queryable<T>().WhereIF(whereExpression != null, whereExpression).AnyAsync();
        }

        #endregion

        #region 插入操作

        /// <summary>
        /// 插入实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>插入后的实体</returns>
        public virtual async Task<T> InsertAsync(T entity)
        {
            var id = await _db.Insertable(entity).ExecuteReturnIdentityAsync();
            entity.Id = Convert.ToInt64(id);
            return entity;
        }

        /// <summary>
        /// 批量插入实体
        /// </summary>
        /// <param name="entities">实体列表</param>
        /// <returns>插入成功的数量</returns>
        public virtual async Task<int> InsertRangeAsync(List<T> entities)
        {
            var result = await _db.Insertable(entities).ExecuteCommandAsync();
            return result;
        }

        #endregion

        #region 更新操作

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>更新后的实体</returns>
        public virtual async Task<T> UpdateAsync(T entity)
        {
            await _db.Updateable(entity).ExecuteCommandAsync();
            return entity;
        }

        /// <summary>
        /// 根据主键更新指定字段
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="columns">要更新的字段</param>
        /// <returns>是否更新成功</returns>
        public virtual async Task<bool> UpdateAsync(TKey id, Dictionary<string, object> columns)
        {
            var updateable = _db.Updateable<T>().Where(it => it.Id.Equals(id));

            // 逐个设置要更新的列
            foreach (var column in columns)
            {
                updateable = updateable.SetColumns(column.Key, column.Value);
            }

            var result = await updateable.ExecuteCommandAsync();
            return result > 0;
        }

        /// <summary>
        /// 批量更新实体
        /// </summary>
        /// <param name="entities">实体列表</param>
        /// <returns>更新成功的数量</returns>
        public virtual async Task<int> UpdateRangeAsync(List<T> entities)
        {
            var result = await _db.Updateable(entities).ExecuteCommandAsync();
            return result;
        }

        /// <summary>
        /// 根据条件批量更新实体
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="columns">要更新的字段</param>
        /// <returns>更新成功的数量</returns>
        public virtual async Task<int> UpdateRangeAsync(Expression<Func<T, bool>> whereExpression, Dictionary<string, object> columns)
        {
            var updateable = _db.Updateable<T>().WhereIF(whereExpression != null, whereExpression);

            // 逐个设置要更新的列
            foreach (var column in columns)
            {
                updateable = updateable.SetColumns(column.Key, column.Value);
            }

            var result = await updateable.ExecuteCommandAsync();
            return result;
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
            // 如果实体实现了软删除接口，则执行软删除
            if (typeof(ISoftDelete).IsAssignableFrom(typeof(T)))
            {
                var result = await _db.Updateable<T>()
                    .SetColumns(it => new T { IsDeleted = true, DeleteTime = DateTime.Now })
                    .Where(it => it.Id.Equals(id))
                    .ExecuteCommandAsync();
                return result > 0;
            }
            else
            {
                // 否则执行物理删除
                var result = await _db.Deleteable<T>().In(id).ExecuteCommandAsync();
                return result > 0;
            }
        }

        /// <summary>
        /// 根据条件批量删除实体（软删除）
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <returns>删除成功的数量</returns>
        public virtual async Task<int> DeleteAsync(Expression<Func<T, bool>> whereExpression)
        {
            int result;
            // 如果实体实现了软删除接口，则执行软删除
            if (typeof(ISoftDelete).IsAssignableFrom(typeof(T)))
            {
                result = await _db.Updateable<T>()
                    .SetColumns(it => new T { IsDeleted = true, DeleteTime = DateTime.Now })
                    .WhereIF(whereExpression != null, whereExpression)
                    .ExecuteCommandAsync();
            }
            else
            {
                // 否则执行物理删除
                result = await _db.Deleteable<T>()
                    .WhereIF(whereExpression != null, whereExpression)
                    .ExecuteCommandAsync();
            }
            return result;
        }
        #endregion
    }

    /// <summary>
    /// 默认主键为long的仓储实现
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    public class Repository<T> : Repository<T, long>, IRepository<T> where T : BaseEntity, new()
    {
        public Repository(ISqlSugarClient db) : base(db)
        {
        }
    }
}