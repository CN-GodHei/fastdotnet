
using Fastdotnet.Core.Entities.Sys;

namespace Fastdotnet.Service.Initializers
{
    /// <summary>
    /// 字典类型数据初始化器
    /// </summary>
    public class FdDictDataInitializer : IApplicationInitializer
    {
        private readonly IRepository<FdDictData, string> _repository;

        public FdDictDataInitializer(
            IRepository<FdDictData, string> repository
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

            var fdDictDataEntries = new List<FdDictData>
                {
                new FdDictData{ Id="11921994044146601", DictTypeId="11921994044146601",Code="CODE_01_01", Label="输入框", Value="Input", OrderNo=100, Remark="输入框", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146602", DictTypeId="11921994044146601",Code="CODE_01_02", Label="字典选择器", Value="DictSelector", OrderNo=100, Remark="字典选择器", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146603", DictTypeId="11921994044146601",Code="CODE_01_03", Label="常量选择器", Value="ConstSelector", OrderNo=100, Remark="常量选择器", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146604", DictTypeId="11921994044146601",Code="CODE_01_04", Label="枚举选择器", Value="EnumSelector", OrderNo=100, Remark="枚举选择器", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146605", DictTypeId="11921994044146601",Code="CODE_01_05", Label="树选择器", Value="ApiTreeSelector", OrderNo=100, Remark="树选择器", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146606", DictTypeId="11921994044146601",Code="CODE_01_06", Label="外键", Value="ForeignKey", OrderNo=100, Remark="外键", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146607", DictTypeId="11921994044146601",Code="CODE_01_07", Label="数字输入框", Value="InputNumber", OrderNo=100, Remark="数字输入框", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146608", DictTypeId="11921994044146601",Code="CODE_01_08", Label="时间选择", Value="DatePicker", OrderNo=100, Remark="时间选择", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146609", DictTypeId="11921994044146601",Code="CODE_01_09", Label="文本域", Value="InputTextArea", OrderNo=100, Remark="文本域", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146610", DictTypeId="11921994044146601",Code="CODE_01_10", Label="上传", Value="Upload", OrderNo=100, Remark="上传", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146611", DictTypeId="11921994044146601",Code="CODE_01_11", Label="开关", Value="Switch", OrderNo=100, Remark="开关", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146612", DictTypeId="11921994044146601",Code="CODE_01_12", Label="上传单文件", Value="Upload_SingleFile", OrderNo=120, Remark="上传单文件", Status=StatusEnum.Enable },

                new FdDictData{ Id="11921994044146613", DictTypeId="11921994044146602",Code="CODE_02_01", Label="等于", Value="==", OrderNo=1, Remark="等于", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146614", DictTypeId="11921994044146602",Code="CODE_02_02", Label="模糊", Value="like", OrderNo=1, Remark="模糊", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146615", DictTypeId="11921994044146602",Code="CODE_02_03", Label="大于", Value=">", OrderNo=1, Remark="大于", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146616", DictTypeId="11921994044146602",Code="CODE_02_04", Label="小于", Value="<", OrderNo=1, Remark="小于", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146617", DictTypeId="11921994044146602",Code="CODE_02_05", Label="不等于", Value="!=", OrderNo=1, Remark="不等于", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146618", DictTypeId="11921994044146602",Code="CODE_02_06", Label="大于等于", Value=">=", OrderNo=1, Remark="大于等于", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146619", DictTypeId="11921994044146602",Code="CODE_02_07", Label="小于等于", Value="<=", OrderNo=1, Remark="小于等于", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146620", DictTypeId="11921994044146602",Code="CODE_02_08", Label="不为空", Value="isNotNull", OrderNo=1, Remark="不为空", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146621", DictTypeId="11921994044146602",Code="CODE_02_09", Label="时间范围", Value="~", OrderNo=1, Remark="时间范围", Status=StatusEnum.Enable },

                new FdDictData{ Id="11921994044146622", DictTypeId="11921994044146603",Code="CODE_03_01", Label="long", Value="long", OrderNo=1, Remark="long", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146623", DictTypeId="11921994044146603",Code="CODE_03_02", Label="string", Value="string", OrderNo=1, Remark="string", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146624", DictTypeId="11921994044146603",Code="CODE_03_03", Label="DateTime", Value="DateTime", OrderNo=1, Remark="DateTime", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146625", DictTypeId="11921994044146603",Code="CODE_03_04", Label="bool", Value="bool", OrderNo=1, Remark="bool", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146626", DictTypeId="11921994044146603",Code="CODE_03_05", Label="int", Value="int", OrderNo=1, Remark="int", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146627", DictTypeId="11921994044146603",Code="CODE_03_06", Label="double", Value="double", OrderNo=1, Remark="double", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146628", DictTypeId="11921994044146603",Code="CODE_03_07", Label="float", Value="float", OrderNo=1, Remark="float", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146629", DictTypeId="11921994044146603",Code="CODE_03_08", Label="decimal", Value="decimal", OrderNo=1, Remark="decimal", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146630", DictTypeId="11921994044146603",Code="CODE_03_09", Label="Guid", Value="Guid", OrderNo=1, Remark="Guid", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146631", DictTypeId="11921994044146603",Code="CODE_03_10", Label="DateTimeOffset", Value="DateTimeOffset", OrderNo=1, Remark="DateTimeOffset", Status=StatusEnum.Enable },

                new FdDictData{ Id="11921994044146632", DictTypeId="11921994044146604",Code="CODE_04_01", Label="下载压缩包", Value="100", OrderNo=1, Remark="下载压缩包", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146633", DictTypeId="11921994044146604",Code="CODE_04_02", Label="下载压缩包(前端)", Value="111", OrderNo=2, Remark="下载压缩包(前端)", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146634", DictTypeId="11921994044146604",Code="CODE_04_03", Label="下载压缩包(后端)", Value="121", OrderNo=3, Remark="下载压缩包(后端)", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146635", DictTypeId="11921994044146604",Code="CODE_04_04", Label="生成到本项目", Value="200", OrderNo=4, Remark="生成到本项目", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146636", DictTypeId="11921994044146604",Code="CODE_04_05", Label="生成到本项目(前端)", Value="211", OrderNo=5, Remark="生成到本项目(前端)", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146637", DictTypeId="11921994044146604",Code="CODE_04_06", Label="生成到本项目(后端)", Value="221", OrderNo=6, Remark="生成到本项目(后端)", Status=StatusEnum.Enable },

                new FdDictData{ Id="11921994044146638", DictTypeId="11921994044146605",Code="CODE_05_01", Label="EntityBaseId【基础实体Id】", Value="EntityBaseId", OrderNo=1, Remark="【基础实体Id】", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146639", DictTypeId="11921994044146605",Code="CODE_05_02", Label="EntityBase【基础实体】", Value="EntityBase", OrderNo=1, Remark="【基础实体】", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146640", DictTypeId="11921994044146605",Code="CODE_05_03", Label="EntityBaseDel【基础软删除实体】", Value="EntityBaseDel", OrderNo=1, Remark="【基础软删除实体】", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146641", DictTypeId="11921994044146605",Code="CODE_05_04", Label="EntityBaseOrg【机构实体】", Value="EntityBaseOrg", OrderNo=1, Remark="【机构实体】", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146642", DictTypeId="11921994044146605",Code="CODE_05_05", Label="EntityBaseOrgDel【机构软删除实体】", Value="EntityBaseOrgDel", OrderNo=1, Remark="【机构软删除实体】", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146643", DictTypeId="11921994044146605",Code="CODE_05_06", Label="EntityBaseTenantId【租户实体Id】", Value="EntityBaseTenantId", OrderNo=1, Remark="【租户实体Id】", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146644", DictTypeId="11921994044146605",Code="CODE_05_07", Label="EntityBaseTenant【租户实体】", Value="EntityBaseTenant", OrderNo=1, Remark="【租户实体】", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146645", DictTypeId="11921994044146605",Code="CODE_05_08", Label="EntityBaseTenantDel【租户软删除实体】", Value="EntityBaseTenantDel", OrderNo=1, Remark="【租户软删除实体】", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146646", DictTypeId="11921994044146605",Code="CODE_05_09", Label="EntityBaseTenantOrg【租户机构实体】", Value="EntityBaseTenantOrg", OrderNo=1, Remark="【租户机构实体】", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146647", DictTypeId="11921994044146605",Code="CODE_05_10", Label="EntityBaseTenantOrgDel【租户机构软删除实体】", Value="EntityBaseTenantOrgDel", OrderNo=1, Remark="【租户机构软删除实体】", Status=StatusEnum.Enable },

                new FdDictData{ Id="11921994044146648", DictTypeId="11921994044146606",Code="CODE_06_01", Label="不需要", Value="off", OrderNo=100, Remark="不需要打印支持", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146649", DictTypeId="11921994044146606",Code="CODE_06_02", Label="绑定打印模版", Value="custom", OrderNo=101, Remark="绑定打印模版", Status=StatusEnum.Enable },

                new FdDictData{ Id="11921994044146650", DictTypeId="11921994044146607",Code="CODE_07_01", Label="集团", Value="101", OrderNo=100, Remark="集团", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146651", DictTypeId="11921994044146607",Code="CODE_07_02", Label="公司", Value="201", OrderNo=101, Remark="公司", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146652", DictTypeId="11921994044146607",Code="CODE_07_03", Label="部门", Value="301", OrderNo=102, Remark="部门", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146653", DictTypeId="11921994044146607",Code="CODE_07_04", Label="区域", Value="401", OrderNo=103, Remark="区域", Status=StatusEnum.Enable },
                new FdDictData{ Id="11921994044146654", DictTypeId="11921994044146607",Code="CODE_07_05", Label="组", Value="501", OrderNo=104, Remark="组", Status=StatusEnum.Enable },

                new FdDictData{ Id="11921994044146655", DictTypeId="11921994044146608",Code="CODE_08_01", Label="用户注册", Value="UserRegister", OrderNo=105, Remark="用户注册", Status=StatusEnum.Enable },
                new FdDictData{ Id="12458431967462405", DictTypeId="11921994044146609",Code="CODE_09_01", Label="站点域名", Value="http://127.0.0.1:18889", OrderNo=105, Remark="站点域名", Status=StatusEnum.Enable },
            };
            foreach (var item in fdDictDataEntries)
            {
                item.Name = item.Label;
            }
            await _repository.InsertRangeAsync(fdDictDataEntries);
        }
    }
}