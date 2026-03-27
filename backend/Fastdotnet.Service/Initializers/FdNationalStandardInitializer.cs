using Fastdotnet.Core.Entities.Sys;

namespace Fastdotnet.Service.Initializers
{
    /// <summary>
    /// 国家标准数据初始化器
    /// </summary>
    public class FdNationalStandardInitializer : IApplicationInitializer
    {
        private readonly IRepository<FdNationalStandard, string> _standardRepository;
        private readonly IRepository<FdNationalStandardItem, string> _itemRepository;

        public FdNationalStandardInitializer(
            IRepository<FdNationalStandard, string> standardRepository,
            IRepository<FdNationalStandardItem, string> itemRepository)
        {
            _standardRepository = standardRepository;
            _itemRepository = itemRepository;
        }

        public async Task InitializeAsync()
        {
            // 如果已有数据，跳过初始化
            var allStandards = await _standardRepository.GetListAsync(x => true);
            if (allStandards.Any())
            {
                return;
            }

            // 示例：初始化 GB/T 2260 行政区划代码（简化版）
            var gb2260 = new FdNationalStandard
            {
                StandardCode = "GB/T 2260",
                StandardName = "中华人民共和国行政区划代码",
                StandardType = "GB/T",
                PublishDepartment = "国家市场监督管理总局",
                PublishDate = new DateTime(2023, 1, 1),
                ImplementDate = new DateTime(2023, 6, 1),
                CurrentVersion = "2023",
                Status = true,
                TotalItems = 0
            };

            var items = new List<FdNationalStandardItem>
            {
                // 省级行政区示例
                new FdNationalStandardItem
                {
                    StandardId = "", // 会在插入后自动填充
                    ItemCode = "110000",
                    ItemName = "北京市",
                    ParentCode = null,
                    Level = 1,
                    Sort = 1,
                    Status = true
                },
                new FdNationalStandardItem
                {
                    StandardId = "",
                    ItemCode = "120000",
                    ItemName = "天津市",
                    ParentCode = null,
                    Level = 1,
                    Sort = 2,
                    Status = true
                },
                new FdNationalStandardItem
                {
                    StandardId = "",
                    ItemCode = "310000",
                    ItemName = "上海市",
                    ParentCode = null,
                    Level = 1,
                    Sort = 3,
                    Status = true
                },
                new FdNationalStandardItem
                {
                    StandardId = "",
                    ItemCode = "500000",
                    ItemName = "重庆市",
                    ParentCode = null,
                    Level = 1,
                    Sort = 4,
                    Status = true
                },
                // 市级示例（北京市辖区）
                new FdNationalStandardItem
                {
                    StandardId = "",
                    ItemCode = "110100",
                    ItemName = "北京城区",
                    ParentCode = "110000",
                    Level = 2,
                    Sort = 1,
                    Status = true
                },
                new FdNationalStandardItem
                {
                    StandardId = "",
                    ItemCode = "110101",
                    ItemName = "东城区",
                    ParentCode = "110100",
                    Level = 3,
                    Sort = 1,
                    Status = true
                },
                new FdNationalStandardItem
                {
                    StandardId = "",
                    ItemCode = "110102",
                    ItemName = "西城区",
                    ParentCode = "110100",
                    Level = 3,
                    Sort = 2,
                    Status = true
                }
            };

            // 更新总数
            gb2260.TotalItems = items.Count;

            // 批量插入（Id 由 SqlSugar AOP 自动填充）
            await _standardRepository.InsertAsync(gb2260);
            
            // 设置正确的 StandardId
            foreach (var item in items)
            {
                item.StandardId = gb2260.Id;
            }
            
            await _itemRepository.InsertRangeAsync(items);
        }
    }
}
