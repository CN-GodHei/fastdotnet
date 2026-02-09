using System;
using System.Collections.Generic;
using System.Text;

namespace Fastdotnet.Service.IService.Sys
{
    /// <summary>
    /// 角色初始化需要在其他初始化之前初始化，做成服务单独调用，避免循环依赖
    /// </summary>
    public interface IFdRoleInitializerService
    {
        Task RoleInitializer();
    }
}
