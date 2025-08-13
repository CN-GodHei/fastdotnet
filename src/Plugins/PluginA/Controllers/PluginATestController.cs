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
        // 4. POST /api/plugintest/page/search - 根据条件分页获取PluginATest实体
        // 5. POST /api/plugintest - 创建新的PluginATest实体
        // 6. PUT /api/plugintest/{id} - 更新指定ID的PluginATest实体
        // 7. DELETE /api/plugintest/{id} - 删除指定ID的PluginATest实体
        // 8. DELETE /api/plugintest/batch - 批量删除PluginATest实体
        
        // 继承自GenericControllerBase的回收站相关操作:
        // 9. GET /api/plugintest/recyclebin - 获取回收站中的PluginATest实体
        // 10. POST /api/plugintest/recyclebin/search - 根据条件查询回收站中的PluginATest实体
        // 11. PUT /api/plugintest/recyclebin/{id}/restore - 恢复回收站中的PluginATest实体
        // 12. POST /api/plugintest/recyclebin/restore - 批量恢复回收站中的PluginATest实体
        // 13. DELETE /api/plugintest/recyclebin/{id}/permanent - 永久删除回收站中的PluginATest实体
        // 14. POST /api/plugintest/recyclebin/permanent - 根据条件永久删除回收站中的PluginATest实体

        // 可以在此处添加PluginATest特有的业务方法
    }
}