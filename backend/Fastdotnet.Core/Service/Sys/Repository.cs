using Microsoft.AspNetCore.Http.HttpResults;

namespace Fastdotnet.Core.Service.Sys
{
    /// <summary>
    /// 通用仓储实现
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <typeparam name="TKey">主键类型</typeparam>
    public class Repository<T, TKey> : IRepository<T, TKey> where T : BaseEntity, new()
    {
        protected readonly ISqlSugarClient _db;

        /// <summary>
        /// 获取数据库客户端实例
        /// </summary>
        public ISqlSugarClient Db => _db;

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
        public virtual async Task<T> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return await _db.Queryable<T>().InSingleAsync(id);
        }

        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <returns>实体列表</returns>
        public virtual async Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.Queryable<T>().ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 根据条件查询实体列表
        /// </summary>
        /// <param name="whereExpression">查询条件表达式</param>
        /// <returns>实体列表</returns>
        public virtual async Task<List<T>> GetListAsync(Expression<Func<T, bool>> whereExpression, CancellationToken cancellationToken = default)
        {
            return await _db.Queryable<T>().WhereIF(whereExpression != null, whereExpression).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 根据条件查询第一个实体
        /// </summary>
        /// <param name="whereExpression">查询条件表达式</param>
        /// <returns>实体对象</returns>
        public virtual async Task<T> GetFirstAsync(Expression<Func<T, bool>> whereExpression, CancellationToken cancellationToken = default)
        {
            return await _db.Queryable<T>().WhereIF(whereExpression != null, whereExpression).FirstAsync(cancellationToken);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="orderByExpression">排序表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns>分页结果</returns>
        public virtual async Task<PageResult<T>> GetPageAsync(
            int pageIndex,
            int pageSize,
            Expression<Func<T, object>> orderByExpression = null,
            OrderByType orderByType = OrderByType.Asc, CancellationToken cancellationToken = default)
        {
            return await GetPageAsync(null, pageIndex, pageSize, orderByExpression, orderByType, cancellationToken);
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
        public virtual async Task<PageResult<T>> GetPageAsync(
            Expression<Func<T, bool>> whereExpression,
            int pageIndex,
            int pageSize,
            Expression<Func<T, object>> orderByExpression = null,
            OrderByType orderByType = OrderByType.Asc, CancellationToken cancellationToken = default)
        {
            RefAsync<int> totalCount = 0;
            var query = _db.Queryable<T>().WhereIF(whereExpression != null, whereExpression);

            if (orderByExpression != null)
            {
                query = query.OrderBy(orderByExpression, orderByType);
            }
            var list = await query.ToPageListAsync(pageIndex, pageSize, totalCount, cancellationToken);
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
            //var id = await _db.Insertable(entity).ExecuteReturnIdentityAsync();
            await _db.Insertable(entity).ExecuteCommandAsync();
            //entity.Id = Convert.ToInt64(id);
            return entity;
        }

        /// <summary>
        /// 批量插入实体
        /// </summary>
        /// <param name="entities">实体列表</param>
        /// <returns>插入成功的数量</returns>
        public virtual async Task<int> InsertRangeAsync(List<T> entities)
        {
            // 在批量插入前，为所有实体生成ID和设置创建时间
            // 这样可以确保每个实体都有唯一ID，避免AOP事件在批量操作中可能的问题
            //foreach (var entity in entities)
            //{
            //    if (string.IsNullOrEmpty(entity.Id))
            //    {
            //        entity.Id = SnowflakeIdGenerator.NextStrId();
            //    }

            //    if (entity.CreatedAt == default(DateTime))
            //    {
            //        entity.CreatedAt = DateTime.Now;
            //    }
            //}

            var insertable = _db.Insertable(entities);

            // 使用SqlSugar的批量插入方法
            var result = await insertable.ExecuteCommandAsync();
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

        #region Upsert操作

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
                return await _db.Deleteable<T>().In(id).IsLogic().ExecuteCommandAsync("IsDeleted", true, "DeletedAt") > 0;
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
                // 使用 Updateable 进行软删除
                var updateable = _db.Updateable<T>()
                    .SetColumns(it => new T { IsDeleted = true, DeletedAt = DateTime.Now });

                // 只有当条件表达式不为 null 时，才添加 WHERE 条件
                if (whereExpression != null)
                {
                    updateable = updateable.Where(whereExpression);
                    result = await updateable.ExecuteCommandAsync();
                }
                else
                {
                    // 如果没有条件，抛出异常防止意外更新整个表
                    throw new ArgumentException("软删除操作必须提供条件表达式，以防止意外更新整个表。");
                }
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

        #region 回收站操作

        /// <summary>
        /// 获取回收站数据（已删除但未被物理删除的数据）
        /// </summary>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="orderByExpression">排序表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns>分页结果</returns>
        public virtual async Task<PageResult<T>> GetRecycleBinAsync(
            int pageIndex,
            int pageSize,
            Expression<Func<T, object>> orderByExpression = null,
            OrderByType orderByType = OrderByType.Desc, CancellationToken cancellationToken = default)
        {
            // 查询已删除的数据
            Expression<Func<T, bool>> whereExpression = entity => entity.IsDeleted;

            RefAsync<int> totalCount = 0;
            var query = _db.Queryable<T>().WhereIF(whereExpression != null, whereExpression);

            if (orderByExpression != null)
            {
                query = query.OrderBy(orderByExpression, orderByType);
            }
            else
            {
                // 默认按删除时间倒序排列
                query = query.OrderBy(entity => entity.DeletedAt, OrderByType.Desc);
            }

            var list = await query.ToPageListAsync(pageIndex, pageSize, totalCount, cancellationToken);
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
        /// 根据条件查询回收站数据
        /// </summary>
        /// <param name="whereExpression">查询条件表达式</param>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="orderByExpression">排序表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns>分页结果</returns>
        public virtual async Task<PageResult<T>> GetRecycleBinAsync(
            Expression<Func<T, bool>> whereExpression,
            int pageIndex,
            int pageSize,
            Expression<Func<T, object>> orderByExpression = null,
            OrderByType orderByType = OrderByType.Desc, CancellationToken cancellationToken = default)
        {
            // 合并条件：必须是已删除的数据，并且满足传入的条件
            Expression<Func<T, bool>> deletedExpression = entity => entity.IsDeleted;
            var combinedExpression = CombineExpressions(deletedExpression, whereExpression);

            RefAsync<int> totalCount = 0;
            var query = _db.Queryable<T>().WhereIF(combinedExpression != null, combinedExpression);

            if (orderByExpression != null)
            {
                query = query.OrderBy(orderByExpression, orderByType);
            }
            else
            {
                // 默认按删除时间倒序排列
                query = query.OrderBy(entity => entity.DeletedAt, OrderByType.Desc);
            }

            var list = await query.ToPageListAsync(pageIndex, pageSize, totalCount, cancellationToken);
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
        /// 恢复回收站中的实体
        /// </summary>
        /// <param name="id">实体主键</param>
        /// <returns>是否恢复成功</returns>
        public virtual async Task<bool> RestoreAsync(TKey id)
        {
            // 只有实现了软删除接口的实体才能恢复
            if (typeof(ISoftDelete).IsAssignableFrom(typeof(T)))
            {
                var result = await _db.Updateable<T>()
                    .SetColumns(it => new T
                    {
                        IsDeleted = false,
                        DeletedAt = null,
                        UpdatedAt = DateTime.Now
                    })
                    .Where(it => it.Id.Equals(id) && it.IsDeleted)
                    .ExecuteCommandAsync();
                return result > 0;
            }
            return false;
        }

        /// <summary>
        /// 批量恢复回收站中的实体
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <returns>恢复成功的数量</returns>
        public virtual async Task<int> RestoreAsync(Expression<Func<T, bool>> whereExpression)
        {
            // 只有实现了软删除接口的实体才能恢复
            if (typeof(ISoftDelete).IsAssignableFrom(typeof(T)))
            {
                // 合并条件：必须是已删除的数据，并且满足传入的条件
                Expression<Func<T, bool>> deletedExpression = entity => entity.IsDeleted;
                var combinedExpression = CombineExpressions(deletedExpression, whereExpression);

                var result = await _db.Updateable<T>()
                    .SetColumns(it => new T
                    {
                        IsDeleted = false,
                        DeletedAt = null,
                        UpdatedAt = DateTime.Now
                    })
                    .WhereIF(combinedExpression != null, combinedExpression)
                    .ExecuteCommandAsync();
                return result;
            }
            return 0;
        }

        /// <summary>
        /// 永久删除回收站中的实体
        /// </summary>
        /// <param name="id">实体主键</param>
        /// <returns>是否删除成功</returns>
        public virtual async Task<bool> PermanentDeleteAsync(TKey id)
        {
            var result = await _db.Deleteable<T>().In(id).ExecuteCommandAsync();
            return result > 0;
        }

        /// <summary>
        /// 根据条件永久删除回收站中的实体
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <returns>删除成功的数量</returns>
        public virtual async Task<int> PermanentDeleteAsync(Expression<Func<T, bool>> whereExpression)
        {
            // 合并条件：必须是已删除的数据，并且满足传入的条件
            Expression<Func<T, bool>> deletedExpression = entity => entity.IsDeleted;
            var combinedExpression = CombineExpressions(deletedExpression, whereExpression);

            var result = await _db.Deleteable<T>()
                .WhereIF(combinedExpression != null, combinedExpression)
                .ExecuteCommandAsync();
            return result;
        }

        #endregion

        #region 私有辅助方法

        /// <summary>
        /// 合并两个表达式（AND关系）
        /// </summary>
        /// <param name="expr1">第一个表达式</param>
        /// <param name="expr2">第二个表达式</param>
        /// <returns>合并后的表达式</returns>
        private Expression<Func<T, bool>> CombineExpressions(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            if (expr1 == null) return expr2;
            if (expr2 == null) return expr1;

            var parameter = Expression.Parameter(typeof(T), "entity");
            var body = Expression.AndAlso(
                Expression.Invoke(expr1, parameter),
                Expression.Invoke(expr2, parameter)
            );
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        public async Task<List<TResult>> GetListAsync<TResult>(
            Expression<Func<T, bool>> whereExpression,
            Expression<Func<T, TResult>> selectExpression,
            CancellationToken ct = default)
        {
            return await _db.Queryable<T>()
                .Where(whereExpression)
                .Select(selectExpression)
                .ToListAsync(ct);
        }

        public async Task<List<Dictionary<string, object>>> GetProjectedListAsync(
    Expression<Func<T, bool>>? whereExpression,
    string[] selectFields,
    CancellationToken ct = default)
        {
            if (selectFields == null || selectFields.Length == 0)
                throw new ArgumentException("SelectFields cannot be null or empty.");

            // 字段白名单校验
            var validProperties = typeof(T).GetProperties()
                .Where(p => p.CanRead)
                .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

            var validatedColumns = new List<string>();
            foreach (var field in selectFields)
            {
                if (validProperties.TryGetValue(field, out var propInfo))
                {
                    validatedColumns.Add(propInfo.Name); // 保留实体中的真实属性名（大小写）
                }
                // 不再 throw，而是静默忽略无效字段
            }

            // 如果过滤后没有有效字段，可以决定是否报错或返回空列表
            if (validatedColumns.Count == 0)
            {
                // 选项1：返回空列表（无字段可查）
                return new List<Dictionary<string, object>>();
            }

            // 构建查询
            var query = _db.Queryable<T>();
            if (whereExpression != null)
                query = query.Where(whereExpression);
            // 构建 Select 字符串
            string selectClause = string.Join(", ", validatedColumns);
            // 关键：只选择指定列
            query = query.Select(selectClause);

            var entities = await query.ToListAsync(ct);

            // 转为字典
            return entities.Select(entity =>
            {
                var dict = new Dictionary<string, object>();
                foreach (var prop in validatedColumns)
                {
                    var value = typeof(T).GetProperty(prop)?.GetValue(entity);
                    dict[prop] = value ?? DBNull.Value;
                }
                return dict;
            }).ToList();
        }
        #endregion

        public async Task<bool> HardDeleteAsync(TKey id)
        {
            var result = await _db.Deleteable<T>().In(id).ExecuteCommandAsync();
            return result > 0;
        }

        //public async Task<int> HardDeleteAsync(Expression<Func<T, bool>> whereExpression)
        //{
        //    var result = await _db.Deleteable<T>()
        //         .WhereIF(whereExpression != null, whereExpression)
        //         .ExecuteCommandAsync();
        //    return result;
        //}
        public async Task<int> HardDeleteAsync(Expression<Func<T, bool>> whereExpression)
        {
            if (whereExpression == null)
            {
                //避免删全表
                return 0;
            }

            // 2. 只有当条件存在时，才执行删除
            var result = await _db.Deleteable<T>()
                 .Where(whereExpression) // 直接使用 Where，因为上面已经确保不为空
                 .ExecuteCommandAsync();

            return result;
        }
    }

    /// <summary>
    /// 默认主键为string的仓储实现
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    public class Repository<T> : Repository<T, string>, IRepository<T> where T : BaseEntity, new()
    {
        public Repository(ISqlSugarClient db) : base(db)
        {
        }
    }
}