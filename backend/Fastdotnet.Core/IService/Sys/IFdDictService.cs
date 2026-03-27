using Fastdotnet.Core.Dtos.Sys;
using Fastdotnet.Core.Entities.Sys;

namespace Fastdotnet.Core.IService.Sys
{
    /// <summary>
    /// 字典数据服务接口
    /// </summary>
    public interface IFdDictService
    {
        /// <summary>
        /// 保存字典数据（幂等操作，只插入不存在的项）
        /// </summary>
        /// <param name="dictTypeAndDataList">字典类型和数据列表</param>
        /// <returns></returns>
        Task SaveDictDataAsync(List<DictTypeAndData> dictTypeAndDataList);
    }
}
