using Fastdotnet.Core.Controllers;
using Fastdotnet.Core.IService;
using Microsoft.AspNetCore.Mvc;
using PluginA.Entities;

namespace PluginA.Controllers
{
    /// <summary>
    /// PluginATest实体的控制器
    /// 继承自通用控制器基类，实现基本的CRUD操作
    /// </summary>
    [Route("api/[controller]")]
    public class PluginATestController : GenericControllerBase<PluginATest>
    {
        private readonly IRepository<PluginATest> _pluginATestRepository;

        public PluginATestController(IRepository<PluginATest> pluginATestRepository) : base(pluginATestRepository)
        {
            _pluginATestRepository = pluginATestRepository;
        }

        // 继承自GenericControllerBase的通用CRUD操作:
        // 1. GET /api/plugintest - 获取所有PluginATest实体
        // 2. GET /api/plugintest/{id} - 根据ID获取PluginATest实体
        // 3. GET /api/plugintest/page - 分页获取PluginATest实体
        // 4. POST /api/plugintest - 创建新的PluginATest实体
        // 5. PUT /api/plugintest/{id} - 更新指定ID的PluginATest实体
        // 6. DELETE /api/plugintest/{id} - 删除指定ID的PluginATest实体
        // 7. DELETE /api/plugintest/batch - 批量删除PluginATest实体

        // 可以在此处添加PluginATest特有的业务方法
    }
}