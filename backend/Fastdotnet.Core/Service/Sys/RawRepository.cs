using Fastdotnet.Core.Dtos.Base;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.IService.Sys;
using Fastdotnet.Core.Services.System;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Fastdotnet.Core.Service.Sys
{
    public class RawRepository<T, TKey> : IRawRepository<T, TKey>
        where T : class, new()
    {
        protected readonly ISqlSugarClient _db;

        /// <summary>
        /// 获取数据库客户端实例
        /// </summary>
        public ISqlSugarClient Db => _db;

        public RawRepository(ISqlSugarClient db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        #region 查询

        public virtual async Task<T?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return await _db.Queryable<T>().InSingleAsync(id);
        }

        public virtual async Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.Queryable<T>().ToListAsync(cancellationToken);
        }

        public virtual async Task<List<T>> GetListAsync(Expression<Func<T, bool>> whereExpression, CancellationToken cancellationToken = default)
        {
            return await _db.Queryable<T>().Where(whereExpression).ToListAsync(cancellationToken);
        }

        public virtual async Task<T?> GetFirstAsync(Expression<Func<T, bool>> whereExpression, CancellationToken cancellationToken = default)
        {
            return await _db.Queryable<T>().Where(whereExpression).FirstAsync(cancellationToken);
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> whereExpression, CancellationToken cancellationToken = default)
        {
            return await _db.Queryable<T>().Where(whereExpression).AnyAsync(cancellationToken);
        }

        public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await _db.Queryable<T>().CountAsync(cancellationToken);
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> whereExpression, CancellationToken cancellationToken = default)
        {
            return await _db.Queryable<T>().Where(whereExpression).CountAsync(cancellationToken);
        }

        #endregion

        #region 插入

        public virtual async Task<T> InsertAsync(T entity, CancellationToken cancellationToken = default)
        {
            var result = await _db.Insertable(entity).ExecuteReturnEntityAsync();
            return result;
        }

        public virtual async Task<int> InsertRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            if (entities == null) return 0;
            var list = entities.ToList();
            if (!list.Any()) return 0;
            return await _db.Insertable(list).ExecuteCommandAsync(cancellationToken);
        }

        #endregion

        #region 更新

        public virtual async Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            return await _db.Updateable(entity).ExecuteCommandHasChangeAsync(cancellationToken);
        }

        public virtual async Task<int> UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            if (entities == null) return 0;
            var list = entities.ToList();
            if (!list.Any()) return 0;
            return await _db.Updateable(list).ExecuteCommandAsync(cancellationToken);
        }

        public virtual async Task<int> UpdateRangeAsync(
            Expression<Func<T, bool>> whereExpression,
            Dictionary<string, object> columns,
            CancellationToken cancellationToken = default)
        {
            if (columns == null || !columns.Any())
                return 0;

            var updateable = _db.Updateable<T>().Where(whereExpression);
            foreach (var col in columns)
            {
                updateable = updateable.SetColumns(col.Key, col.Value);
            }
            return await updateable.ExecuteCommandAsync(cancellationToken);
        }

        #endregion

        #region 删除

        public virtual async Task<bool> DeleteAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return await _db.Deleteable<T>().In(id).ExecuteCommandHasChangeAsync();
        }

        public virtual async Task<bool> DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            return await _db.Deleteable(entity).ExecuteCommandHasChangeAsync();
        }

        public virtual async Task<int> DeleteRangeAsync(Expression<Func<T, bool>> whereExpression, CancellationToken cancellationToken = default)
        {
            return await _db.Deleteable<T>().Where(whereExpression).ExecuteCommandAsync(cancellationToken);
        }

        /// <summary>
        /// 插入或更新实体（Upsert操作）
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>操作后的实体</returns>
        public virtual async Task<T> InsertOrUpdateAsync(T entity)
        {
            var result = await _db.Storageable(entity).ExecuteCommandAsync();
            return entity;
        }

        /// <summary>
        /// 通过主键进行Upsert操作
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>操作后的实体</returns>
        public virtual async Task<T> AddAndUpdateItByPrimaryKeysAsync(T entity)
        {
            // SqlSugar的Storageable方法可以实现upsert功能，基于主键判断
            var result = await _db.Storageable(entity).ExecuteCommandAsync();
            return entity;
        }
        #endregion
    }
    /// <summary>
    /// 默认主键为string的仓储实现
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    public class RawRepository<T> : RawRepository<T, string>, IRawRepository<T> where T : class, new()
    {
        public RawRepository(ISqlSugarClient db) : base(db)
        {
        }
    }
}