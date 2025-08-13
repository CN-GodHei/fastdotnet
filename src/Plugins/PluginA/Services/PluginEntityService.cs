using Fastdotnet.Core.IService;
using Fastdotnet.Core.Models;
using Fastdotnet.Core.Service;
using Microsoft.Extensions.Logging;
using PluginA.Entities;
using PluginA.IService;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PluginA.Services
{
    /// <summary>
    /// 插件实体服务实现
    /// </summary>
    public class PluginEntityService : Repository<PluginEntity, long>, IPluginEntityService
    {
        private readonly ILogger<PluginEntityService> _logger;

        public PluginEntityService(ISqlSugarClient sqlSugarClient, ILogger<PluginEntityService> logger) : base(sqlSugarClient)
        {
            _logger = logger;
        }

        /// <summary>
        /// 根据名称分页搜索实体
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>分页结果</returns>
        public async Task<PageResult<PluginEntity>> SearchByNamePagedAsync(string name, int pageIndex, int pageSize)
        {
            Expression<Func<PluginEntity, bool>> whereExpression = x => true;
            if (!string.IsNullOrEmpty(name))
            {
                whereExpression = x => x.Name.Contains(name);
            }

            return await GetPageAsync(whereExpression, pageIndex, pageSize);
        }

        /// <summary>
        /// 根据名称搜索实体
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>实体列表</returns>
        public async Task<List<PluginEntity>> SearchByNameAsync(string name)
        {
            return await GetListAsync(it => it.Name.Contains(name));
        }

        /// <summary>
        /// 获取最近创建的实体
        /// </summary>
        /// <param name="count">数量</param>
        /// <returns>实体列表</returns>
        public async Task<List<PluginEntity>> GetRecentAsync(int count = 10)
        {
            return await _db.Queryable<PluginEntity>()
                .Where(it => it.IsDeleted == false)
                .OrderBy(it => it.CreateTime, OrderByType.Desc)
                .Take(count)
                .Select(it => new PluginEntity
                {
                    Id = it.Id,
                    Name = it.Name,
                    Description = it.Description,
                    CreateTime = it.CreateTime
                })
                .ToListAsync();
        }
    }
}