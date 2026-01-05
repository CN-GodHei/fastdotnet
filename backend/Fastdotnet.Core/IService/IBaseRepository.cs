using Fastdotnet.Core.Dtos.Base;
using Fastdotnet.Core.IService;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Fastdotnet.Core.IService
{
    /// <summary>
    /// 长整型主键仓储接口
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    public interface IBaseRepository<T> : IRepository<T, long> where T : BaseEntity, new()
    {
    }
}