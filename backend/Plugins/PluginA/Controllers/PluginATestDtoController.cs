using AutoMapper;
using Fastdotnet.Core.Controllers;
using Fastdotnet.Core.IService;
using Microsoft.AspNetCore.Mvc;
using NetTaste;
using PluginA.Dto;
using PluginA.Entities;

namespace PluginA.Controllers
{
    // PluginATest的DTO类定义




    /// <summary>
    /// PluginATest实体的DTO控制器
    /// 继承自通用DTO控制器基类，实现基于DTO的CRUD操作
    /// </summary>
    [Route("api/[controller]")]
    public class PluginATestDtoController : GenericDtoControllerBase<PluginATest, string, PluginATestCreateDto, PluginATestUpdateDto, PluginATestDto>
    {
        public PluginATestDtoController(IBaseService<PluginATest, string> pluginATestService, IMapper mapper) 
            : base(pluginATestService, mapper)
        {
        }

        // 继承自GenericDtoControllerBase的通用CRUD操作:
        // 1. GET /api/plugintestdto - 获取所有PluginATest实体并转换为PluginATestDto
        // 2. GET /api/plugintestdto/{id} - 根据ID获取PluginATest实体并转换为PluginATestDto
        // 3. GET /api/plugintestdto/page - 分页获取PluginATest实体并转换为PluginATestDto
        // 4. POST /api/plugintestdto/page/search - 根据条件分页获取PluginATest实体并转换为PluginATestDto
        // 5. POST /api/plugintestdto - 创建新的PluginATest实体（从PluginATestCreateDto转换）
        // 6. PUT /api/plugintestdto/{id} - 更新指定ID的PluginATest实体（从PluginATestUpdateDto转换）
        // 7. DELETE /api/plugintestdto/{id} - 删除指定ID的PluginATest实体
        // 8. DELETE /api/plugintestdto/batch - 批量删除PluginATest实体

        // 继承自GenericDtoControllerBase的回收站相关操作:
        // 9. GET /api/plugintestdto/recyclebin - 获取回收站中的PluginATest实体并转换为PluginATestDto
        // 10. POST /api/plugintestdto/recyclebin/search - 根据条件查询回收站中的PluginATest实体并转换为PluginATestDto
        // 11. PUT /api/plugintestdto/recyclebin/{id}/restore - 恢复回收站中的PluginATest实体
        // 12. POST /api/plugintestdto/recyclebin/restore - 批量恢复回收站中的PluginATest实体
        // 13. DELETE /api/plugintestdto/recyclebin/{id}/permanent - 永久删除回收站中的PluginATest实体
        // 14. POST /api/plugintestdto/recyclebin/permanent - 根据条件永久删除回收站中的PluginATest实体

        // 可以在此处添加PluginATest特有的业务方法

        /// <summary>
        /// 这是一个测试方法
        /// </summary>
        /// <returns></returns>
        [HttpGet("test")]
        public async Task<string> Test()
        {
            return "这是一个测试方法";
        }
    }
}