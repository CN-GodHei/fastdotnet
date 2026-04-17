
using System.Linq.Dynamic.Core;
using Fastdotnet.Core.Dtos.Sys;
using Fastdotnet.Core.Entities.Sys;
using Fastdotnet.Core.IService.Sys;
using Fastdotnet.Service.IService.Sys;

namespace Fastdotnet.Service.Service.Sys
{
    /// <summary>
    /// 字典数据初始化服务实现
    /// </summary>
    public class FdDictService : IFdDictService
    {
        private readonly IRepository<FdDictType, string> _typeRepository;
        private readonly IRepository<FdDictData, string> _dataRepository;

        public FdDictService(
            IRepository<FdDictType, string> typeRepository,
            IRepository<FdDictData, string> dataRepository
            )
        {
            _typeRepository = typeRepository;
            _dataRepository = dataRepository;
        }

        /// <summary>
        /// 保存字典数据（幂等操作，只插入不存在的项）
        /// </summary>
        /// <param name="dictTypeAndDataList">字典类型和数据列表</param>
        public async Task SaveDictDataAsync(List<DictTypeAndData> dictTypeAndDataList)
        {
            // 如果传入数据为空，直接返回
            if (dictTypeAndDataList == null || !dictTypeAndDataList.Any())
            {
                return;
            }

            // 先获取数据库中所有现有的数据
            var existingTypes = await _typeRepository.GetAllAsync();
            var existingData = await _dataRepository.GetAllAsync();
            
            // 准备要插入的数据列表
            var typesToInsert = new List<FdDictType>();
            var dataToInsert = new List<FdDictData>();
            
            // 预处理：按类型分组现有数据，便于快速查找
            var existingDataByType = existingData
                .GroupBy(d => d.DictTypeCode)
                .ToDictionary(g => g.Key, g => g.ToList());
            
            // 预处理：计算每个类型下已有的最大 Code 序号
            var maxSuffixByType = new Dictionary<string, int>();
            foreach (var typeCode in dictTypeAndDataList.Select(x => x.fdDictType.Code).Distinct())
            {
                if (!string.IsNullOrEmpty(typeCode) && existingDataByType.TryGetValue(typeCode, out var dataList))
                {
                    maxSuffixByType[typeCode] = dataList
                        .Select(d => d.Code)
                        .Where(c => !string.IsNullOrEmpty(c) && c.StartsWith($"{typeCode}_"))
                        .Select(c => 
                        {
                            var suffix = c.Substring((typeCode + "_").Length);
                            return int.TryParse(suffix, out int num) ? num : 0;
                        })
                        .DefaultIfEmpty(0)
                        .Max();
                }
                else
                {
                    maxSuffixByType[typeCode] = 0;
                }
            }
            
            // 对比并筛选出需要插入的数据
            foreach (var item in dictTypeAndDataList)
            {
                var typeToAdd = item.fdDictType;
                
                // 检查字典类型是否存在，不存在则可能需要生成 Code
                if (!existingTypes.Any(t => t.Id == item.fdDictType.Id))
                {
                    // 如果类型没有 Code，自动生成
                    if (string.IsNullOrEmpty(typeToAdd.Code))
                    {
                        // 提取 Name 中的 CODE_XX 部分或生成新 Code
                        var codeMatch = System.Text.RegularExpressions.Regex.Match(typeToAdd.Name, @"CODE_(\d+)");
                        if (codeMatch.Success)
                        {
                            typeToAdd.Code = $"CODE_{codeMatch.Groups[1].Value}";
                        }
                        else
                        {
                            // 获取现有 Code 最大值并生成新的
                            var maxCodeNum = existingTypes
                                .Select(t => t.Code)
                                .Where(c => !string.IsNullOrEmpty(c) && c.StartsWith("CODE_"))
                                .Select(c => 
                                {
                                    var match = System.Text.RegularExpressions.Regex.Match(c, @"CODE_(\d+)");
                                    return match.Success ? int.Parse(match.Groups[1].Value) : 0;
                                })
                                .DefaultIfEmpty(0)
                                .Max();
                            
                            typeToAdd.Code = $"CODE_{(maxCodeNum + 1):D2}";
                        }
                    }
                    
                    typesToInsert.Add(typeToAdd);
                }
                else
                {
                    // 类型已存在，使用数据库中的类型信息
                    typeToAdd = existingTypes.First(t => t.Id == item.fdDictType.Id);
                }
                
                // 检查字典数据是否存在，并自动生成 Code
                if (item.fdDictData != null)
                {
                    var typeCode = typeToAdd.Code;
                    var currentSuffix = maxSuffixByType.GetValueOrDefault(typeCode, 0) + 1;
                    
                    foreach (var data in item.fdDictData.Where(d => !existingData.Any(ed => ed.Id == d.Id)))
                    {
                        // 如果没有 Code 或 Code 已存在，自动生成
                        if (string.IsNullOrEmpty(data.Code))
                        {
                            data.Code = $"{typeCode}_{currentSuffix:D3}";
                            currentSuffix++;
                        }
                        
                        dataToInsert.Add(data);
                    }
                }
            }
            
            // 批量插入不存在的字典类型和字典数据
            if (typesToInsert.Any())
            {
                await _typeRepository.InsertRangeAsync(typesToInsert);
            }
            
            if (dataToInsert.Any())
            {
                await _dataRepository.InsertRangeAsync(dataToInsert);
            }
        }
    }
}
