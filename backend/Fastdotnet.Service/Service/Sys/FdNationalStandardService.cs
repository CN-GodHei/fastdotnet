using Fastdotnet.Core.Dtos.Sys;
using Fastdotnet.Core.Entities.Sys;
using Fastdotnet.Core.IService.Sys;

namespace Fastdotnet.Service.Service.Sys
{
    /// <summary>
    /// 国家标准数据服务实现
    /// </summary>
    public class FdNationalStandardService : IFdNationalStandardService
    {
        private readonly IRepository<FdNationalStandard, string> _standardRepository;
        private readonly IRepository<FdNationalStandardItem, string> _itemRepository;

        public FdNationalStandardService(
            IRepository<FdNationalStandard, string> standardRepository,
            IRepository<FdNationalStandardItem, string> itemRepository)
        {
            _standardRepository = standardRepository;
            _itemRepository = itemRepository;
        }

        /// <summary>
        /// 导入标准及其条目（批量操作）
        /// </summary>
        public async Task<string> ImportStandardAsync(FdNationalStandard standard, List<FdNationalStandardItem> items)
        {
            // 1. 检查标准是否已存在
            var existing = await _standardRepository.GetFirstAsync(x => x.StandardCode == standard.StandardCode);
            if (existing != null)
            {
                throw new Exception($"标准 {standard.StandardCode} 已存在，如需更新请使用版本升级功能");
            }

            // 2. 插入标准主表（Id 由 SqlSugar AOP 自动填充）
            standard.TotalItems = items?.Count ?? 0;
            await _standardRepository.InsertAsync(standard);

            // 3. 批量插入条目（Id 由 SqlSugar AOP 自动填充）
            if (items != null && items.Any())
            {
                // StandardId 已在调用前设置
                await _itemRepository.InsertRangeAsync(items);
            }

            return standard.Id;
        }

        /// <summary>
        /// 获取标准的完整树形结构
        /// </summary>
        public async Task<TreeModel<FdNationalStandardItemDto>> GetStandardTreeAsync(string standardCode)
        {
            // 获取标准主表
            var standard = await _standardRepository.GetFirstAsync(x => x.StandardCode == standardCode && x.Status == true);
            if (standard == null)
            {
                throw new Exception($"标准 {standardCode} 不存在或已废止");
            }

            // 获取所有条目
            var allItems = await _itemRepository.GetListAsync(x => 
                x.StandardId == standard.Id && 
                x.Status == true);

            // 构建树形结构
            var itemDtos = allItems.Select(x => new FdNationalStandardItemDto
            {
                Id = x.Id,
                StandardId = x.StandardId,
                ItemCode = x.ItemCode,
                ItemName = x.ItemName,
                ParentCode = x.ParentCode,
                Level = x.Level,
                Sort = x.Sort
            }).ToList();

            var tree = BuildTree(itemDtos, null);
            return new TreeModel<FdNationalStandardItemDto>
            {
                TreeData = tree,
                Total = allItems.Count
            };
        }

        /// <summary>
        /// 根据标准编码获取标准详情（包含条目统计）
        /// </summary>
        public async Task<FdNationalStandardDetailDto> GetStandardDetailAsync(string standardCode)
        {
            var standard = await _standardRepository.GetFirstAsync(x => x.StandardCode == standardCode);
            if (standard == null)
            {
                throw new Exception($"标准 {standardCode} 不存在");
            }

            // 统计条目数量
            var itemCountList = await _itemRepository.GetListAsync(x => x.StandardId == standard.Id);

            return new FdNationalStandardDetailDto
            {
                Id = standard.Id,
                StandardCode = standard.StandardCode,
                StandardName = standard.StandardName,
                StandardType = standard.StandardType,
                CurrentVersion = standard.CurrentVersion,
                Status = standard.Status,
                TotalItems = itemCountList.Count,
                PublishDate = standard.PublishDate,
                ImplementDate = standard.ImplementDate
            };
        }

        /// <summary>
        /// 更新标准版本（版本升级）
        /// </summary>
        public async Task<(string oldVersionId, string newVersionId)> UpdateVersionAsync(
            string standardCode, 
            string newVersion, 
            List<FdNationalStandardItem> newItems)
        {
            // 1. 获取当前版本
            var oldStandard = await _standardRepository.GetFirstAsync(x => 
                x.StandardCode == standardCode && x.Status == true);

            if (oldStandard == null)
            {
                throw new Exception($"标准 {standardCode} 不存在");
            }

            var oldVersionId = oldStandard.Id;

            // 2. 标记旧版本为历史
            oldStandard.Status = false;
            await _standardRepository.UpdateAsync(oldStandard);

            // 3. 创建新版本（Id 由 SqlSugar AOP 自动填充）
            var newStandard = new FdNationalStandard
            {
                StandardCode = standardCode,
                StandardName = oldStandard.StandardName,
                StandardType = oldStandard.StandardType,
                CurrentVersion = newVersion,
                PublishDate = DateTime.Now,
                Status = true,
                TotalItems = newItems?.Count ?? 0
            };
            await _standardRepository.InsertAsync(newStandard);

            // 4. 插入新版本的条目（Id 由 SqlSugar AOP 自动填充）
            if (newItems != null && newItems.Any())
            {
                // StandardId 已在调用前设置
                await _itemRepository.InsertRangeAsync(newItems);
            }

            return (oldVersionId, newStandard.Id);
        }

        #region 辅助方法

        /// <summary>
        /// 递归构建树形结构
        /// </summary>
        private List<FdNationalStandardItemDto> BuildTree(List<FdNationalStandardItemDto> allNodes, string? parentCode)
        {
            var nodes = allNodes.Where(x => x.ParentCode == parentCode).ToList();
            foreach (var node in nodes)
            {
                node.Children = BuildTree(allNodes, node.ItemCode);
            }
            return nodes;
        }

        #endregion
    }
}
