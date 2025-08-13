using Fastdotnet.Core.IService;
using Fastdotnet.Core.Models;
using Fastdotnet.Core.Models.Base;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Fastdotnet.Core.Controllers
{
    /// <summary>
    /// 通用控制器基类，提供基本的CRUD操作
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TKey">主键类型</typeparam>
    [ApiController]
    public abstract class GenericControllerBase<TEntity, TKey> : ControllerBase 
        where TEntity : BaseEntity, new()
        where TKey : IEquatable<TKey>
    {
        protected readonly IRepository<TEntity, TKey> _repository;

        protected GenericControllerBase(IRepository<TEntity, TKey> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <returns>实体列表</returns>
        [HttpGet]
        public virtual async Task<List<TEntity>> GetAll()
        {
            return await _repository.GetAllAsync();
        }

        /// <summary>
        /// 根据ID获取实体
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <returns>实体对象</returns>
        [HttpGet("{id}")]
        public virtual async Task<TEntity> GetById(TKey id)
        {
            return await _repository.GetByIdAsync(id);
        }

        /// <summary>
        /// 分页获取实体
        /// </summary>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>分页结果</returns>
        [HttpGet("page")]
        public virtual async Task<PageResult<TEntity>> GetPage([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            return await _repository.GetPageAsync(pageIndex, pageSize);
        }

        /// <summary>
        /// 根据条件分页获取实体
        /// </summary>
        /// <param name="whereExpression">查询条件表达式</param>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>分页结果</returns>
        [HttpPost("page/search")]
        public virtual async Task<PageResult<TEntity>> GetPageByCondition(
            [FromBody] Expression<Func<TEntity, bool>> whereExpression,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            return await _repository.GetPageAsync(whereExpression, pageIndex, pageSize);
        }

        /// <summary>
        /// 创建实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>创建后的实体</returns>
        [HttpPost]
        public virtual async Task<TEntity> Create(TEntity entity)
        {
            // 可以在子类中重写BeforeCreate方法来添加自定义逻辑
            await BeforeCreate(entity);
            
            var result = await _repository.InsertAsync(entity);
            
            // 可以在子类中重写AfterCreate方法来添加自定义逻辑
            await AfterCreate(result);
            
            return result;
        }

        /// <summary>
        /// 创建实体前的钩子方法，可在子类中重写以添加自定义逻辑
        /// </summary>
        /// <param name="entity">要创建的实体</param>
        protected virtual Task BeforeCreate(TEntity entity)
        {
            // 默认实现为空，子类可以重写添加自定义逻辑
            return Task.CompletedTask;
        }

        /// <summary>
        /// 创建实体后的钩子方法，可在子类中重写以添加自定义逻辑
        /// </summary>
        /// <param name="entity">已创建的实体</param>
        protected virtual Task AfterCreate(TEntity entity)
        {
            // 默认实现为空，子类可以重写添加自定义逻辑
            return Task.CompletedTask;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <param name="entity">实体对象</param>
        /// <returns>更新后的实体</returns>
        [HttpPut("{id}")]
        public virtual async Task<TEntity> Update(TKey id, TEntity entity)
        {
            if (!id.Equals(entity.Id))
            {
                throw new ArgumentException("ID不匹配");
            }

            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                throw new ArgumentException("未找到指定实体");
            }

            // 可以在子类中重写BeforeUpdate方法来添加自定义逻辑
            await BeforeUpdate(existing, entity);
            
            var result = await _repository.UpdateAsync(entity);
            
            // 可以在子类中重写AfterUpdate方法来添加自定义逻辑
            await AfterUpdate(result);
            
            return result;
        }

        /// <summary>
        /// 更新实体前的钩子方法，可在子类中重写以添加自定义逻辑
        /// </summary>
        /// <param name="existing">数据库中已存在的实体</param>
        /// <param name="updated">更新后的实体</param>
        protected virtual Task BeforeUpdate(TEntity existing, TEntity updated)
        {
            // 默认实现为空，子类可以重写添加自定义逻辑
            // 例如：检查权限、验证数据等
            return Task.CompletedTask;
        }

        /// <summary>
        /// 更新实体后的钩子方法，可在子类中重写以添加自定义逻辑
        /// </summary>
        /// <param name="entity">已更新的实体</param>
        protected virtual Task AfterUpdate(TEntity entity)
        {
            // 默认实现为空，子类可以重写添加自定义逻辑
            // 例如：发送通知、更新缓存等
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
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                return false;
            }

            // 可以在子类中重写BeforeDelete方法来添加自定义逻辑
            await BeforeDelete(existing);
            
            var result = await _repository.DeleteAsync(id);
            
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
            return await _repository.DeleteAsync(whereExpression);
        }

        #region 回收站相关接口

        /// <summary>
        /// 获取回收站数据（已软删除的数据）
        /// </summary>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>分页结果</returns>
        [HttpGet("recyclebin")]
        public virtual async Task<PageResult<TEntity>> GetRecycleBin([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            return await _repository.GetRecycleBinAsync(pageIndex, pageSize);
        }

        /// <summary>
        /// 根据条件查询回收站数据
        /// </summary>
        /// <param name="whereExpression">查询条件表达式</param>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>分页结果</returns>
        [HttpPost("recyclebin/search")]
        public virtual async Task<PageResult<TEntity>> SearchRecycleBin(
            [FromBody] Expression<Func<TEntity, bool>> whereExpression,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            return await _repository.GetRecycleBinAsync(whereExpression, pageIndex, pageSize);
        }

        /// <summary>
        /// 恢复回收站中的实体
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <returns>是否恢复成功</returns>
        [HttpPut("recyclebin/{id}/restore")]
        public virtual async Task<bool> Restore(TKey id)
        {
            return await _repository.RestoreAsync(id);
        }

        /// <summary>
        /// 批量恢复回收站中的实体
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <returns>恢复成功的数量</returns>
        [HttpPost("recyclebin/restore")]
        public virtual async Task<int> RestoreBatch([FromBody] Expression<Func<TEntity, bool>> whereExpression)
        {
            return await _repository.RestoreAsync(whereExpression);
        }

        /// <summary>
        /// 永久删除回收站中的实体
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <returns>是否删除成功</returns>
        [HttpDelete("recyclebin/{id}/permanent")]
        public virtual async Task<bool> PermanentDelete(TKey id)
        {
            return await _repository.PermanentDeleteAsync(id);
        }

        /// <summary>
        /// 根据条件永久删除回收站中的实体
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <returns>删除成功的数量</returns>
        [HttpPost("recyclebin/permanent")]
        public virtual async Task<int> PermanentDeleteBatch([FromBody] Expression<Func<TEntity, bool>> whereExpression)
        {
            return await _repository.PermanentDeleteAsync(whereExpression);
        }

        #endregion
    }

    /// <summary>
    /// 通用控制器基类，默认主键为long
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    public abstract class GenericControllerBase<TEntity> : GenericControllerBase<TEntity, long> 
        where TEntity : BaseEntity, new()
    {
        protected GenericControllerBase(IRepository<TEntity, long> repository) : base(repository)
        {
        }
    }
}