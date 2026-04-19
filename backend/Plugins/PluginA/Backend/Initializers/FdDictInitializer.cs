using Fastdotnet.Core.Dtos.Sys;
using Fastdotnet.Core.Entities.Sys;
using Fastdotnet.Core.Initializers;
using Fastdotnet.Core.IService.Sys;
using Fastdotnet.Core.Plugin;
using Fastdotnet.Core.Utils;
using Fastdotnet.Plugin.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plugina.Initializers
{
    /// <summary>
    /// 字典数据初始化器（合并类型和数据）
    /// </summary>
    public class FdDictInitializer : IApplicationInitializer
    {
        private readonly IFdDictService _dictInitializerService;

        public FdDictInitializer(IFdDictService dictInitializerService)
        {
            _dictInitializerService = dictInitializerService;
        }

        public async Task InitializeAsync()
        {
            // 使用 DictTypeAndData 格式组织数据
            var dictTypeAndDataList = new List<DictTypeAndData>
            {
                 new DictTypeAndData
                {
                    fdDictType = new FdDictType
                    {
                        //Name = "插件分类",
                        //Code = "01",
                        //SysFlag = YesNoEnum.Y,
                        //OrderNo = 201,
                        //Remark = "用户相关配置",
                        //Status = StatusEnum.Enable
                    },
                    fdDictData = new List<FdDictData>
                    {
                        //new FdDictData{ Code="01", Label="行业应用", Value="1", ValueType=DictValueType.String, OrderNo=1, Remark="", Status=StatusEnum.Enable },
                        //new FdDictData{ Code="02", Label="企业应用", Value="2", ValueType=DictValueType.String, OrderNo=2, Remark="", Status=StatusEnum.Enable },
                        //new FdDictData{ Code="03", Label="小程序", Value="3", ValueType=DictValueType.String, OrderNo=3, Remark="", Status=StatusEnum.Enable },
                        //new FdDictData{ Code="04", Label="Uni-app", Value="4", ValueType=DictValueType.String, OrderNo=4, Remark="", Status=StatusEnum.Enable },
                        //new FdDictData{ Code="05", Label="编辑器", Value="5", ValueType=DictValueType.String, OrderNo=5, Remark="", Status=StatusEnum.Enable },
                        //new FdDictData{ Code="06", Label="云存储", Value="6", ValueType=DictValueType.String, OrderNo=6, Remark="", Status=StatusEnum.Enable },
                        //new FdDictData{ Code="07", Label="短信验证", Value="7", ValueType=DictValueType.String, OrderNo=7, Remark="", Status=StatusEnum.Enable },
                        //new FdDictData{ Code="08", Label="接口整合", Value="8", ValueType=DictValueType.String, OrderNo=8, Remark="", Status=StatusEnum.Enable },
                        //new FdDictData{ Code="09", Label="开发测试", Value="9", ValueType=DictValueType.String, OrderNo=9, Remark="", Status=StatusEnum.Enable },
                        //new FdDictData{ Code="10", Label="未归类", Value="10", ValueType=DictValueType.String, OrderNo=10, Remark="", Status=StatusEnum.Enable },
                    }
                }};
            var pluginInfo = PluginContext.GetCurrentPluginInfo();

            // 遍历生成字典类型ID并关联到字典数据
            foreach (var item in dictTypeAndDataList)
            {
                if (item.fdDictType.Code == null)
                {
                    item.fdDictType = null;
                    item.fdDictData = null;
                    continue;
                }
                // 为字典类型生成唯一ID
                string typeId = SnowflakeIdGenerator.NextStrId();
                item.fdDictType.Id = typeId;
                item.fdDictType.PluginId = pluginInfo?.id;
                item.fdDictType.Code = pluginInfo?.id + "_CODE_" + "_" + item.fdDictType.Code;
                // 将生成的ID关联到所有字典数据项
                foreach (var dictData in item.fdDictData)
                {
                    dictData.Id = SnowflakeIdGenerator.NextStrId();
                    dictData.DictTypeId = typeId;
                    dictData.DictTypeCode = item.fdDictType.Code;
                    dictData.Code = item.fdDictType.Code + "_" + dictData.Code;
                }
            }

            // 调用服务保存字典数据
            await _dictInitializerService.SaveDictDataAsync(dictTypeAndDataList);
        }
    }
}