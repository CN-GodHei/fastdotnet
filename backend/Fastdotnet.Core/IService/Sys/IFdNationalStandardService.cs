using Fastdotnet.Core.Entities.Sys;

namespace Fastdotnet.Core.IService.Sys
{
    /// <summary>
    /// 国家标准数据服务接口
    /// </summary>
    public interface IFdNationalStandardService
    {
        /// <summary>
        /// 导入标准及其条目（批量操作）
        /// </summary>
        /// <param name="standard">标准主表数据</param>
        /// <param name="items">标准条目列表</param>
        /// <returns></returns>
        Task<string> ImportStandardAsync(FdNationalStandard standard, List<FdNationalStandardItem> items);

        /// <summary>
        /// 获取标准的完整树形结构
        /// </summary>
        /// <param name="standardCode">国标编号</param>
        /// <returns></returns>
        Task<TreeModel<FdNationalStandardItemDto>> GetStandardTreeAsync(string standardCode);

        /// <summary>
        /// 根据标准编码获取标准详情（包含条目统计）
        /// </summary>
        /// <param name="standardCode">国标编号</param>
        /// <returns></returns>
        Task<FdNationalStandardDetailDto> GetStandardDetailAsync(string standardCode);

        /// <summary>
        /// 更新标准版本（版本升级）
        /// </summary>
        /// <param name="standardCode">国标编号</param>
        /// <param name="newVersion">新版本号</param>
        /// <param name="newItems">新版本的条目数据</param>
        /// <returns>新旧版本 ID 对比</returns>
        Task<(string oldVersionId, string newVersionId)> UpdateVersionAsync(
            string standardCode, 
            string newVersion, 
            List<FdNationalStandardItem> newItems);
    }
}
