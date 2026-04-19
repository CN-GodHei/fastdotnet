using Fastdotnet.Core.IService;
using Fastdotnet.Core.Models;
using PluginA.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PluginA.IService
{
    /// <summary>
    /// PluginEntity服务接口
    /// </summary>
    public interface IPluginEntityService : IRepository<PluginEntity>
    {
        /// <summary>
        /// 根据名称搜索实体列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>实体列表</returns>
        Task<List<PluginEntity>> SearchByNameAsync(string name);

        /// <summary>
        /// 根据名称分页搜索实体
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>分页结果</returns>
        Task<PageResult<PluginEntity>> SearchByNamePagedAsync(string name, int pageIndex, int pageSize);
    }
}