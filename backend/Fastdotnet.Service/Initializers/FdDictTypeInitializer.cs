
using Fastdotnet.Core.Entities.Sys;

namespace Fastdotnet.Service.Initializers
{
    /// <summary>
    /// 字典类型数据初始化器
    /// </summary>
    public class FdDictTypeInitializer : IApplicationInitializer
    {
        private readonly IRepository<FdDictType, string> _repository;

        public FdDictTypeInitializer(
            IRepository<FdDictType, string> repository
            )
        {
            _repository = repository;
        }

        public async Task InitializeAsync()
        {

            if (await _repository.ExistsAsync(b => b.Id != null))
            {
                return;
            }

            var fdDictTypeEntries = new List<FdDictType>
            {
                new FdDictType{ Id="11921994044146601", Name="代码生成控件类型", Code="CODE_01", SysFlag=YesNoEnum.Y, OrderNo=100, Remark="代码生成控件类型", Status=StatusEnum.Enable },
                new FdDictType{ Id="11921994044146602", Name="代码生成查询类型", Code="CODE_02", SysFlag=YesNoEnum.Y, OrderNo=101, Remark="代码生成查询类型", Status=StatusEnum.Enable },
                new FdDictType{ Id="11921994044146603", Name="代码生成.NET类型", Code="CODE_03", SysFlag=YesNoEnum.Y, OrderNo=102, Remark="代码生成.NET类型", Status=StatusEnum.Enable },
                new FdDictType{ Id="11921994044146604", Name="代码生成方式", Code="CODE_04", SysFlag=YesNoEnum.Y, OrderNo=103, Remark="代码生成方式", Status=StatusEnum.Enable },
                new FdDictType{ Id="11921994044146605", Name="代码生成基类", Code="CODE_05", SysFlag=YesNoEnum.Y, OrderNo=104, Remark="代码生成基类", Status=StatusEnum.Enable },
                new FdDictType{ Id="11921994044146606", Name="代码生成打印类型", Code="CODE_06", SysFlag=YesNoEnum.Y, OrderNo=105, Remark="代码生成打印类型", Status=StatusEnum.Enable },
                new FdDictType{ Id="11921994044146607", Name="机构类型", Code="CODE_07", SysFlag=YesNoEnum.Y, OrderNo=201, Remark="机构类型", Status=StatusEnum.Enable },
                new FdDictType{ Id="11921994044146608", Name="邮箱操作业务名称", Code="CODE_08", SysFlag=YesNoEnum.Y, OrderNo=201, Remark="业务名称", Status=StatusEnum.Enable },
                new FdDictType{ Id="11921994044146609", Name="系统配置", Code="CODE_09", SysFlag=YesNoEnum.Y, OrderNo=201, Remark="系统配置", Status=StatusEnum.Enable },
            };
            await _repository.InsertRangeAsync(fdDictTypeEntries);
        }
    }
}