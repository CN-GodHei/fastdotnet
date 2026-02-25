

namespace Fastdotnet.Core.Controllers
{
    /// <summary>
    /// 支持 DTO 的App端通用控制器基类
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TKey">主键类型</typeparam>
    /// <typeparam name="TCreateDto">创建 DTO 类型</typeparam>
    /// <typeparam name="TUpdateDto">更新 DTO 类型</typeparam>
    /// <typeparam name="TDto">输出 DTO 类型</typeparam>
    [ApiController]
    [ApiUsageScope(ApiUsageScopeEnum.AppOnly)]
    public abstract class AppGenericDtoControllerBase<TEntity, TKey, TCreateDto, TUpdateDto, TDto> : ControllerBase
        where TEntity : BaseEntity, new()
        where TKey : IEquatable<TKey>
        where TCreateDto : class
        where TUpdateDto : class
    {
        protected readonly IBaseService<TEntity, TKey> _service;
        protected readonly IMapper _mapper;
        protected readonly ICurrentUser _currentUser;

        protected AppGenericDtoControllerBase(IBaseService<TEntity, TKey> service, IMapper mapper, ICurrentUser currentUser)
        {
            _service = service;
            _mapper = mapper;
            _currentUser = currentUser;
        }
        protected AppGenericDtoControllerBase(IBaseService<TEntity, TKey> service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }
        /// <summary>
        /// 获取所有实体
        /// </summary>
        [HttpGet]
        public virtual async Task<List<TDto>> GetAll(CancellationToken cancellationToken = default)
        {
            var entities = await _service.GetAllAsync(cancellationToken);
            return _mapper.Map<List<TDto>>(entities);
        }

        /// <summary>
        /// 根据自定义条件获取列表(不分页、支持只查询特定列)
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("list-by-condition")]
        public async Task<IActionResult> GetListByCondition(
            [FromBody] QueryByConditionDto query,
            CancellationToken cancellationToken = default)
        {
            // 可以在子类中重写BeforeGetListByCondition方法来添加自定义逻辑
            await BeforeGetListByCondition(query);

            if (query.SelectFields.Count() > 0)
            {
                var result = await _service.GetProjectedListByConditionAsync(
                query.DynamicQuery,
                query.QueryParameters,
                query.SelectFields,
                cancellationToken);
                // 可以在子类中重写AfterGetListByCondition方法来添加自定义逻辑
                // 调用 After 钩子并获取可能修改后的结果
                var processedResult = await AfterGetListByCondition(query, result);

                // 如果 After 钩子返回了新的结果，则使用新结果
                if (processedResult != null)
                {
                    return Ok(processedResult);
                }
                return Ok(result);
            }
            else
            {
                // 构建动态表达式
                Expression<Func<TEntity, bool>>? whereExpression = null;

                // 如果有动态查询条件，则构建表达式
                if (!string.IsNullOrEmpty(query.DynamicQuery))
                {
                    whereExpression = DynamicExpressionParser.ParseLambda<TEntity, bool>(
                        ParsingConfig.Default,
                        false,
                        query.DynamicQuery,
                        query.QueryParameters ?? new object[0]);
                }

                var result = await _service.GetListAsync(
                    whereExpression);
                // 可以在子类中重写AfterGetListByCondition方法来添加自定义逻辑
                // 调用 After 钩子并获取可能修改后的结果
                var processedResult = await AfterGetListByCondition(query, result);

                // 如果 After 钩子返回了新的结果，则使用新结果
                if (processedResult != null)
                {
                    return Ok(processedResult);
                }
                return Ok(result);
            }
        }

                /// <summary>
        /// 根据自定义条件获取列表前的钩子方法
        /// </summary>
        /// <param name="query">查询条件</param>
        protected virtual Task BeforeGetListByCondition(QueryByConditionDto query)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 根据自定义条件获取列表后的钩子方法（可返回处理后的值）
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <param name="result">原始查询结果</param>
        /// <returns>处理后的结果（返回null则使用原始结果）</returns>
        protected virtual Task<object?> AfterGetListByCondition(QueryByConditionDto query, object result)
        {
            // 默认实现返回null，表示不修改原始结果
            return Task.FromResult<object?>(null);
        }

        /// <summary>
        /// 根据ID获取实体
        /// </summary>
        /// <param name="id">实体ID</param>
        [HttpGet("{id}")]
        public virtual async Task<TDto> GetById(TKey id, CancellationToken cancellationToken = default)
        {
            var entity = await _service.GetByIdAsync(id);
            return _mapper.Map<TDto>(entity);
        }

        /// <summary>
        /// 分页获取实体
        /// </summary>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">页大小</param>
        [HttpGet("page")]
        public virtual async Task<PageResult<TDto>> GetPage([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var pageResult = await _service.GetPageAsync(pageIndex, pageSize);
            return new PageResult<TDto>
            {
                Items = _mapper.Map<IList<TDto>>(pageResult.Items) ?? new List<TDto>(),
                PageInfo = pageResult.PageInfo,
            };
        }

        /// <summary>
        /// 根据条件分页获取实体 (使用 PageQueryByConditionDto)
        /// </summary>
        /// <param name="query">分页和查询条件</param>
        [HttpPost("page/search")]
        [Consumes("application/json")]
        public virtual async Task<PageResult<TDto>> GetPageByCondition([FromBody] PageQueryByConditionDto query, CancellationToken cancellationToken = default)
        {
            // 构建动态表达式
            Expression<Func<TEntity, bool>>? whereExpression = null;

            // 如果有动态查询条件，则构建表达式
            if (!string.IsNullOrEmpty(query.DynamicQuery))
            {
                whereExpression = DynamicExpressionParser.ParseLambda<TEntity, bool>(
                    ParsingConfig.Default,
                    false,
                    query.DynamicQuery,
                    query.QueryParameters ?? new object[0]);
            }

            var pageResult = await _service.GetPageAsync(
                whereExpression,
                query.PageIndex,
                query.PageSize);

            return new PageResult<TDto>
            {
                Items = _mapper.Map<IList<TDto>>(pageResult.Items) ?? new List<TDto>(),
                PageInfo = pageResult.PageInfo,
            };
        }

        /// <summary>
        /// 创建实体
        /// </summary>
        /// <param name="dto">创建 DTO 对象</param>
        [HttpPost]
        public virtual async Task<TDto> Create(TCreateDto dto)
        {
            dto.IsValid();
            var entity = _mapper.Map<TEntity>(dto);

            // 可以在子类中重写BeforeCreate方法来添加自定义逻辑
            await BeforeCreate(entity, dto);

            var result = await _service.InsertAsync(entity);

            // 可以在子类中重写AfterCreate方法来添加自定义逻辑
            await AfterCreate(result, dto);

            return _mapper.Map<TDto>(result);
        }

        /// <summary>
        /// 创建实体前的钩子方法，可在子类中重写以添加自定义逻辑
        /// </summary>
        /// <param name="entity">要创建的实体</param>
        /// <param name="dto">创建 DTO</param>
        protected virtual Task BeforeCreate(TEntity entity, TCreateDto dto)
        {
            // 默认实现为空，子类可以重写添加自定义逻辑
            return Task.CompletedTask;
        }

        /// <summary>
        /// 创建实体后的钩子方法，可在子类中重写以添加自定义逻辑
        /// </summary>
        /// <param name="entity">已创建的实体</param>
        /// <param name="dto">创建 DTO</param>
        protected virtual Task AfterCreate(TEntity entity, TCreateDto dto)
        {
            // 默认实现为空，子类可以重写添加自定义逻辑
            return Task.CompletedTask;
        }

        /// <summary>
        /// 批量创建实体
        /// </summary>
        /// <param name="dto">创建 DTO 对象</param>
        [HttpPost("batch")]
        public virtual async Task<int> CreateMany([FromBody] List<TCreateDto> dtos)
        {
            if (dtos == null)
            {
                throw new BusinessException("参数不能为空!");
            }
            dtos.IsValid();
            var entitys = _mapper.Map<List<TEntity>>(dtos);

            // 可以在子类中重写BeforeCreate方法来添加自定义逻辑
            await BeforeCreateMany(entitys, dtos);

            var result = await _service.InsertRangeAsync(entitys);

            // 可以在子类中重写AfterCreate方法来添加自定义逻辑
            await AfterCreateMany(entitys, dtos);

            return result;
        }

        /// <summary>
        /// 批量创建实体前的钩子方法，可在子类中重写以添加自定义逻辑
        /// </summary>
        /// <param name="entity">要创建的实体</param>
        /// <param name="dto">创建 DTO</param>
        protected virtual Task BeforeCreateMany(List<TEntity> entitys, List<TCreateDto> dtos)
        {
            // 默认实现为空，子类可以重写添加自定义逻辑
            return Task.CompletedTask;
        }

        /// <summary>
        /// 批量创建实体后的钩子方法，可在子类中重写以添加自定义逻辑
        /// </summary>
        /// <param name="entity">已创建的实体</param>
        /// <param name="dto">创建 DTO</param>
        protected virtual Task AfterCreateMany(List<TEntity> entitys, List<TCreateDto> dtos)
        {
            // 默认实现为空，子类可以重写添加自定义逻辑
            return Task.CompletedTask;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <param name="dto">更新 DTO 对象</param>
        [HttpPut("{id}")]
        public virtual async Task<TDto> Update(TKey id, TUpdateDto dto)
        {
            dto.IsValid();
            var existing = await _service.GetByIdAsync(id);
            if (existing == null)
            {
                throw new ArgumentException("未找到指定实体");
            }

            _mapper.Map(dto, existing);

            // 可以在子类中重写BeforeUpdate方法来添加自定义逻辑
            await BeforeUpdate(existing, existing, dto);

            var result = await _service.UpdateAsync(existing);

            // 可以在子类中重写AfterUpdate方法来添加自定义逻辑
            await AfterUpdate(result, dto);

            return _mapper.Map<TDto>(result);
        }

        /// <summary>
        /// 更新实体前的钩子方法，可在子类中重写以添加自定义逻辑
        /// </summary>
        /// <param name="existing">数据库中已存在的实体</param>
        /// <param name="updated">更新后的实体</param>
        /// <param name="dto">更新 DTO</param>
        protected virtual Task BeforeUpdate(TEntity existing, TEntity updated, TUpdateDto dto)
        {
            // 默认实现为空，子类可以重写添加自定义逻辑
            // 例如：检查权限、验证数据等
            return Task.CompletedTask;
        }

        /// <summary>
        /// 更新实体后的钩子方法，可在子类中重写以添加自定义逻辑
        /// </summary>
        /// <param name="entity">已更新的实体</param>
        /// <param name="dto">更新 DTO</param>
        protected virtual Task AfterUpdate(TEntity entity, TUpdateDto dto)
        {
            // 默认实现为空，子类可以重写添加自定义逻辑
            // 例如：发送通知、更新缓存等
            return Task.CompletedTask;
        }

        /// <summary>
        /// 批量更新实体前的钩子方法，可在子类中重写以添加自定义逻辑
        /// </summary>
        /// <param name="entities">待更新的实体列表</param>
        /// <param name="dtos">更新 DTO 列表</param>
        protected virtual Task BeforeUpdateMany(List<TEntity> entities, List<TUpdateDto> dtos)
        {
            // 默认实现为空，子类可以重写添加自定义逻辑
            return Task.CompletedTask;
        }

        /// <summary>
        /// 批量更新实体后的钩子方法，可在子类中重写以添加自定义逻辑
        /// </summary>
        /// <param name="entities">已更新的实体列表</param>
        /// <param name="dtos">更新 DTO 列表</param>
        protected virtual Task AfterUpdateMany(List<TEntity> entities, List<TUpdateDto> dtos)
        {
            // 默认实现为空，子类可以重写添加自定义逻辑
            return Task.CompletedTask;
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <returns>操作结果</returns>
        [HttpDelete("{id}")]
        public virtual async Task<bool> Delete(TKey id)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null)
            {
                return false;
            }

            // 可以在子类中重写BeforeDelete方法来添加自定义逻辑
            await BeforeDelete(existing);

            var result = await _service.DeleteAsync(id);

            // 可以在子类中重写AfterDelete方法来添加自定义逻辑
            await AfterDelete(id, result);

            return result;
        }

        /// <summary>
        /// 删除实体前的钩子方法，可在子类中重写以添加自定义逻辑
        /// </summary>
        /// <param name="entity">要删除的实体</param>
        protected virtual Task BeforeDelete(TEntity entity)
        {
            // 默认实现为空，子类可以重写添加自定义逻辑
            // 例如：检查是否可以删除、记录日志等
            return Task.CompletedTask;
        }

        /// <summary>
        /// 删除实体后的钩子方法，可在子类中重写以添加自定义逻辑
        /// </summary>
        /// <param name="id">被删除的实体ID</param>
        /// <param name="result">删除操作结果</param>
        protected virtual Task AfterDelete(TKey id, bool result)
        {
            // 默认实现为空，子类可以重写添加自定义逻辑
            // 例如：清理关联数据、发送通知等
            return Task.CompletedTask;
        }

        /// <summary>
        /// 批量删除实体
        /// </summary>
        /// <param name="ids">ID列表</param>
        /// <returns>操作结果</returns>
        [HttpDelete("batch")]
        public virtual async Task<int> BatchDelete([FromBody] List<TKey> ids)
        {
            // 使用object类型来处理不同类型的ID比较
            var idObjects = ids.Cast<object>().ToList();
            Expression<Func<TEntity, bool>> whereExpression = entity => idObjects.Contains(entity.Id);
            return await _service.DeleteAsync(whereExpression);
        }

        /// <summary>
        /// 批量更新实体
        /// </summary>
        /// <param name="dtos">更新 DTO 对象列表</param>
        /// <returns>操作结果</returns>
        [HttpPut("batch")]
        public virtual async Task<int> UpdateMany([FromBody] List<TUpdateDto> dtos)
        {
            if (dtos == null || !dtos.Any())
            {
                throw new BusinessException("参数不能为空!");
            }

            dtos.IsValid();
            var updatedEntities = _mapper.Map<List<TEntity>>(dtos);

            // 可以在子类中重写BeforeUpdateMany方法来添加自定义逻辑
            await BeforeUpdateMany(updatedEntities, dtos);

            var result = await _service.UpdateRangeAsync(updatedEntities);

            // 可以在子类中重写AfterUpdateMany方法来添加自定义逻辑
            await AfterUpdateMany(updatedEntities, dtos);

            return result;
        }

        /// <summary>
        /// 根据条件批量更新实体属性（部分字段更新）
        /// </summary>
        /// <param name="updateInfo">更新信息，包含条件和更新值</param>
        /// <returns>操作结果</returns>
        [HttpPut("batch/updatebycondition")]
        public virtual async Task<int> UpdateManyByCondition([FromBody] BatchUpdateByConditionDto<TUpdateDto> updateInfo)
        {
            if (updateInfo == null)
            {
                throw new BusinessException("参数不能为空!");
            }

            // 使用 AutoMapper 将 DTO 映射为实体，以便提取要更新的字段
            var updateEntity = _mapper.Map<TEntity>(updateInfo.Dto);

            // 获取要更新的字段和值
            var updateColumns = new Dictionary<string, object>();
            var updateEntityProps = typeof(TEntity).GetProperties()
                .Where(p => p.Name != nameof(BaseEntity.Id) && p.CanWrite)
                .ToList();

            foreach (var prop in updateEntityProps)
            {
                var value = prop.GetValue(updateEntity);
                if (value != null)
                {
                    updateColumns.Add(prop.Name, value);
                }
            }

            if (!updateColumns.Any())
            {
                return 0; // 没有需要更新的字段
            }

            // 使用动态查询条件
            Expression<Func<TEntity, bool>>? whereExpression = null;

            // 如果提供了动态查询条件，则构建表达式
            if (!string.IsNullOrEmpty(updateInfo.Query?.DynamicQuery))
            {
                whereExpression = DynamicExpressionParser.ParseLambda<TEntity, bool>(
                    ParsingConfig.Default,
                    false,
                    updateInfo.Query.DynamicQuery,
                    updateInfo.Query.QueryParameters ?? new object[0]);
            }

            // 可以在子类中重写BeforeUpdateMany方法来添加自定义逻辑
            // 这里传入空列表，因为实际更新的实体还未从数据库获取
            await BeforeUpdateMany(new List<TEntity>(), new List<TUpdateDto>());

            // 调用 Repository 的批量更新方法
            var result = await _service.UpdateRangeAsync(whereExpression ?? (_ => true), updateColumns);

            // 可以在子类中重写AfterUpdateMany方法来添加自定义逻辑
            await AfterUpdateMany(new List<TEntity>(), new List<TUpdateDto>());

            return result;
        }

        /// <summary>
        /// 从更新 DTO 中提取 ID 值
        /// </summary>
        /// <param name="dto">更新 DTO</param>
        /// <returns>ID 值</returns>
        private TKey GetIdFromDto(TUpdateDto dto)
        {
            var property = typeof(TUpdateDto).GetProperty("Id");
            if (property == null)
            {
                throw new BusinessException($"更新 DTO {typeof(TUpdateDto).Name} 中未找到 Id 属性");
            }
            var value = property.GetValue(dto);
            return (TKey)(value ?? throw new BusinessException($"更新 DTO 中的 Id 值不能为空"));
        }

        /// <summary>
        /// 根据强类型条件分页获取实体
        /// </summary>
        /// <param name="query">包含强类型查询条件的参数</param>
        //[HttpPost("page/typedsearch")]
        //[Consumes("application/json")]
        //public virtual async Task<PageResult<TDto>> GetPageByTypedCondition1([FromBody] TypedPageQueryDto<TUpdateDto> query)
        //{
        //    Expression<Func<TEntity, bool>>? whereExpression = null;

        //    var expressions = new List<Expression>();

        //    // 使用DTO属性作为条件（传统的相等查询）
        //    if (query.TypedCondition != null)
        //    {
        //        expressions.AddRange(BuildTypedExpressions(query.TypedCondition));
        //    }

        //    // 使用明确的查询条件列表
        //    if (query.Conditions != null && query.Conditions.Any())
        //    {
        //        expressions.AddRange(BuildConditionExpressions(query.Conditions));
        //    }

        //    if (expressions.Any())
        //    {
        //        // 根据逻辑连接符合并表达式
        //        var combinedExpression = expressions.Aggregate((expr1, expr2) =>
        //            query.Logic.ToLower() == "or" 
        //                ? Expression.OrElse(expr1, expr2) 
        //                : Expression.AndAlso(expr1, expr2));

        //        var parameter = Expression.Parameter(typeof(TEntity), "x");
        //        whereExpression = Expression.Lambda<Func<TEntity, bool>>(combinedExpression, parameter);
        //    }
        //    else
        //    {
        //        // 没有条件时返回所有记录
        //        whereExpression = _ => true;
        //    }

        //    var pageResult = await _repository.GetPageAsync(
        //        whereExpression,
        //        query.PageIndex,
        //        query.PageSize);

        //    return new PageResult<TDto>
        //    {
        //        Items = _mapper.Map<IList<TDto>>(pageResult.Items) ?? new List<TDto>(),
        //        PageInfo = pageResult.PageInfo,
        //    };
        //}

        /// <summary>
        /// 基于DTO构建表达式（所有属性使用相等操作符）
        /// </summary>
        /// <param name="condition">条件 DTO</param>
        /// <returns>查询表达式列表</returns>
        private List<Expression> BuildTypedExpressions(TUpdateDto condition)
        {
            var expressions = new List<Expression>();
            var parameter = Expression.Parameter(typeof(TEntity), "x");

            foreach (var dtoProperty in typeof(TUpdateDto).GetProperties())
            {
                var value = dtoProperty.GetValue(condition);
                if (value == null) continue; // 跳过 null 值

                var entityProperty = typeof(TEntity).GetProperty(dtoProperty.Name);
                if (entityProperty != null && entityProperty.CanRead)
                {
                    var propertyExpression = Expression.Property(parameter, entityProperty);
                    var valueExpression = Expression.Constant(value);
                    var equalExpression = Expression.Equal(propertyExpression, valueExpression);
                    expressions.Add(equalExpression);
                }
            }

            return expressions;
        }

        /// <summary>
        /// 基于条件列表构建表达式
        /// </summary>
        /// <param name="conditions">查询条件列表</param>
        /// <returns>查询表达式列表</returns>
        private List<Expression> BuildConditionExpressions(List<QueryConditionItem> conditions)
        {
            var expressions = new List<Expression>();
            var parameter = Expression.Parameter(typeof(TEntity), "x");

            foreach (var condition in conditions)
            {
                if (string.IsNullOrEmpty(condition.PropertyName)) continue;

                var entityProperty = typeof(TEntity).GetProperty(condition.PropertyName);
                if (entityProperty == null) continue;

                var propertyExpression = Expression.Property(parameter, entityProperty);

                Expression conditionExpression = condition.Operator switch
                {
                    QueryOperator.Equal => Expression.Equal(propertyExpression, Expression.Constant(condition.Value)),
                    QueryOperator.NotEqual => Expression.NotEqual(propertyExpression, Expression.Constant(condition.Value)),
                    QueryOperator.GreaterThan => Expression.GreaterThan(propertyExpression, Expression.Constant(condition.Value)),
                    QueryOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(propertyExpression, Expression.Constant(condition.Value)),
                    QueryOperator.LessThan => Expression.LessThan(propertyExpression, Expression.Constant(condition.Value)),
                    QueryOperator.LessThanOrEqual => Expression.LessThanOrEqual(propertyExpression, Expression.Constant(condition.Value)),
                    QueryOperator.Contains when entityProperty.PropertyType == typeof(string) =>
                        Expression.Call(propertyExpression, typeof(string).GetMethod("Contains", new[] { typeof(string) })!,
                            Expression.Constant(condition.Value)),
                    QueryOperator.StartsWith when entityProperty.PropertyType == typeof(string) =>
                        Expression.Call(propertyExpression, typeof(string).GetMethod("StartsWith", new[] { typeof(string) })!,
                            Expression.Constant(condition.Value)),
                    QueryOperator.EndsWith when entityProperty.PropertyType == typeof(string) =>
                        Expression.Call(propertyExpression, typeof(string).GetMethod("EndsWith", new[] { typeof(string) })!,
                            Expression.Constant(condition.Value)),
                    QueryOperator.Between => BuildBetweenExpression(propertyExpression, condition.Value, condition.Value2),
                    _ => Expression.Equal(propertyExpression, Expression.Constant(condition.Value))
                };

                expressions.Add(conditionExpression);
            }

            return expressions;
        }

        /// <summary>
        /// 构建 BETWEEN 表达式
        /// </summary>
        /// <param name="propertyExpression">属性表达式</param>
        /// <param name="value1">起始值</param>
        /// <param name="value2">结束值</param>
        /// <returns>Between 表达式</returns>
        private Expression BuildBetweenExpression(Expression propertyExpression, object? value1, object? value2)
        {
            if (value1 == null || value2 == null) return Expression.Constant(true);

            var lowerBound = Expression.GreaterThanOrEqual(propertyExpression, Expression.Constant(value1));
            var upperBound = Expression.LessThanOrEqual(propertyExpression, Expression.Constant(value2));

            return Expression.AndAlso(lowerBound, upperBound);
        }

        #region 映射方法


        #endregion
    }

    /// <summary>
    /// 支持 DTO 的通用控制器基类，默认主键为string
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TCreateDto">创建 DTO 类型</typeparam>
    /// <typeparam name="TUpdateDto">更新 DTO 类型</typeparam>
    /// <typeparam name="TDto">输出 DTO 类型</typeparam>
    public abstract class AppGenericDtoControllerBase<TEntity, TCreateDto, TUpdateDto, TDto> : AppGenericDtoControllerBase<TEntity, string, TCreateDto, TUpdateDto, TDto>
        where TEntity : BaseEntity, new()
        where TCreateDto : class
        where TUpdateDto : class
    {
        protected AppGenericDtoControllerBase(IBaseService<TEntity, string> service, IMapper mapper, ICurrentUser currentUser) : base(service, mapper, currentUser)
        {
        }
        protected AppGenericDtoControllerBase(IBaseService<TEntity, string> service, IMapper mapper) : base(service, mapper)
        {
        }
    }
}