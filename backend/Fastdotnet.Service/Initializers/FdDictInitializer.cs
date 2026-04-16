
using Fastdotnet.Core.Dtos.Sys;
using Fastdotnet.Core.Entities.Sys;
using Fastdotnet.Core.IService.Sys;

namespace Fastdotnet.Service.Initializers
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

        /// <summary>
        /// 字典初始化应该最先执行（Order = 1000）
        /// </summary>
        public int Order => 1000;

        public async Task InitializeAsync()
        {
            // 使用 DictTypeAndData 格式组织数据
            var dictTypeAndDataList = new List<DictTypeAndData>
            {
                // CODE_01: 代码生成控件类型
                new DictTypeAndData
                {
                    fdDictType = new FdDictType
                    {
                        Id = "11921994044146601",
                        Name = "代码生成控件类型",
                        Code = "CODE_01",
                        SysFlag = YesNoEnum.Y,
                        OrderNo = 100,
                        Remark = "代码生成控件类型",
                        Status = StatusEnum.Enable
                    },
                    fdDictData = new List<FdDictData>
                    {
                        new FdDictData{ Id="11921994044146601", DictTypeId="11921994044146601", DictTypeCode="CODE_01", Code="CODE_01_01", Label="输入框", Value="Input", ValueType=DictValueType.String, OrderNo=100, Remark="输入框", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146602", DictTypeId="11921994044146601", DictTypeCode="CODE_01", Code="CODE_01_02", Label="字典选择器", Value="DictSelector", ValueType=DictValueType.String, OrderNo=100, Remark="字典选择器", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146603", DictTypeId="11921994044146601", DictTypeCode="CODE_01", Code="CODE_01_03", Label="常量选择器", Value="ConstSelector", ValueType=DictValueType.String, OrderNo=100, Remark="常量选择器", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146604", DictTypeId="11921994044146601", DictTypeCode="CODE_01", Code="CODE_01_04", Label="枚举选择器", Value="EnumSelector", ValueType=DictValueType.String, OrderNo=100, Remark="枚举选择器", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146605", DictTypeId="11921994044146601", DictTypeCode="CODE_01", Code="CODE_01_05", Label="树选择器", Value="ApiTreeSelector", ValueType=DictValueType.String, OrderNo=100, Remark="树选择器", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146606", DictTypeId="11921994044146601", DictTypeCode="CODE_01", Code="CODE_01_06", Label="外键", Value="ForeignKey", ValueType=DictValueType.String, OrderNo=100, Remark="外键", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146607", DictTypeId="11921994044146601", DictTypeCode="CODE_01", Code="CODE_01_07", Label="数字输入框", Value="InputNumber", ValueType=DictValueType.String, OrderNo=100, Remark="数字输入框", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146608", DictTypeId="11921994044146601", DictTypeCode="CODE_01", Code="CODE_01_08", Label="时间选择", Value="DatePicker", ValueType=DictValueType.String, OrderNo=100, Remark="时间选择", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146609", DictTypeId="11921994044146601", DictTypeCode="CODE_01", Code="CODE_01_09", Label="文本域", Value="InputTextArea", ValueType=DictValueType.String, OrderNo=100, Remark="文本域", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146610", DictTypeId="11921994044146601", DictTypeCode="CODE_01", Code="CODE_01_10", Label="上传", Value="Upload", ValueType=DictValueType.String, OrderNo=100, Remark="上传", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146611", DictTypeId="11921994044146601", DictTypeCode="CODE_01", Code="CODE_01_11", Label="开关", Value="Switch", ValueType=DictValueType.String, OrderNo=100, Remark="开关", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146612", DictTypeId="11921994044146601", DictTypeCode="CODE_01", Code="CODE_01_12", Label="上传单文件", Value="Upload_SingleFile", ValueType=DictValueType.String, OrderNo=120, Remark="上传单文件", Status=StatusEnum.Enable },
                    }
                },
                
                // CODE_02: 代码生成查询类型
                new DictTypeAndData
                {
                    fdDictType = new FdDictType
                    {
                        Id = "11921994044146602",
                        Name = "代码生成查询类型",
                        Code = "CODE_02",
                        SysFlag = YesNoEnum.Y,
                        OrderNo = 101,
                        Remark = "代码生成查询类型",
                        Status = StatusEnum.Enable
                    },
                    fdDictData = new List<FdDictData>
                    {
                        new FdDictData{ Id="11921994044146613", DictTypeId="11921994044146602", DictTypeCode="CODE_02", Code="CODE_02_01", Label="等于", Value="==", ValueType=DictValueType.String, OrderNo=1, Remark="等于", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146614", DictTypeId="11921994044146602", DictTypeCode="CODE_02", Code="CODE_02_02", Label="模糊", Value="like", ValueType=DictValueType.String, OrderNo=1, Remark="模糊", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146615", DictTypeId="11921994044146602", DictTypeCode="CODE_02", Code="CODE_02_03", Label="大于", Value=">", ValueType=DictValueType.String, OrderNo=1, Remark="大于", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146616", DictTypeId="11921994044146602", DictTypeCode="CODE_02", Code="CODE_02_04", Label="小于", Value="<", ValueType=DictValueType.String, OrderNo=1, Remark="小于", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146617", DictTypeId="11921994044146602", DictTypeCode="CODE_02", Code="CODE_02_05", Label="不等于", Value="!=", ValueType=DictValueType.String, OrderNo=1, Remark="不等于", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146618", DictTypeId="11921994044146602", DictTypeCode="CODE_02", Code="CODE_02_06", Label="大于等于", Value=">=", ValueType=DictValueType.String, OrderNo=1, Remark="大于等于", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146619", DictTypeId="11921994044146602", DictTypeCode="CODE_02", Code="CODE_02_07", Label="小于等于", Value="<=", ValueType=DictValueType.String, OrderNo=1, Remark="小于等于", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146620", DictTypeId="11921994044146602", DictTypeCode="CODE_02", Code="CODE_02_08", Label="不为空", Value="isNotNull", ValueType=DictValueType.String, OrderNo=1, Remark="不为空", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146621", DictTypeId="11921994044146602", DictTypeCode="CODE_02", Code="CODE_02_09", Label="时间范围", Value="~", ValueType=DictValueType.String, OrderNo=1, Remark="时间范围", Status=StatusEnum.Enable },
                    }
                },
                
                // CODE_03: 代码生成.NET 类型
                new DictTypeAndData
                {
                    fdDictType = new FdDictType
                    {
                        Id = "11921994044146603",
                        Name = "代码生成.NET 类型",
                        Code = "CODE_03",
                        SysFlag = YesNoEnum.Y,
                        OrderNo = 102,
                        Remark = "代码生成.NET 类型",
                        Status = StatusEnum.Enable
                    },
                    fdDictData = new List<FdDictData>
                    {
                        new FdDictData{ Id="11921994044146622", DictTypeId="11921994044146603", DictTypeCode="CODE_03", Code="CODE_03_01", Label="long", Value="long", ValueType=DictValueType.String, OrderNo=1, Remark="long", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146623", DictTypeId="11921994044146603", DictTypeCode="CODE_03", Code="CODE_03_02", Label="string", Value="string", ValueType=DictValueType.String, OrderNo=1, Remark="string", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146624", DictTypeId="11921994044146603", DictTypeCode="CODE_03", Code="CODE_03_03", Label="DateTime", Value="DateTime", ValueType=DictValueType.String, OrderNo=1, Remark="DateTime", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146625", DictTypeId="11921994044146603", DictTypeCode="CODE_03", Code="CODE_03_04", Label="bool", Value="bool", ValueType=DictValueType.String, OrderNo=1, Remark="bool", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146626", DictTypeId="11921994044146603", DictTypeCode="CODE_03", Code="CODE_03_05", Label="int", Value="int", ValueType=DictValueType.String, OrderNo=1, Remark="int", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146627", DictTypeId="11921994044146603", DictTypeCode="CODE_03", Code="CODE_03_06", Label="double", Value="double", ValueType=DictValueType.String, OrderNo=1, Remark="double", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146628", DictTypeId="11921994044146603", DictTypeCode="CODE_03", Code="CODE_03_07", Label="float", Value="float", ValueType=DictValueType.String, OrderNo=1, Remark="float", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146629", DictTypeId="11921994044146603", DictTypeCode="CODE_03", Code="CODE_03_08", Label="decimal", Value="decimal", ValueType=DictValueType.String, OrderNo=1, Remark="decimal", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146630", DictTypeId="11921994044146603", DictTypeCode="CODE_03", Code="CODE_03_09", Label="Guid", Value="Guid", ValueType=DictValueType.String, OrderNo=1, Remark="Guid", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146631", DictTypeId="11921994044146603", DictTypeCode="CODE_03", Code="CODE_03_10", Label="DateTimeOffset", Value="DateTimeOffset", ValueType=DictValueType.String, OrderNo=1, Remark="DateTimeOffset", Status=StatusEnum.Enable },
                    }
                },
                
                // CODE_04: 代码生成方式
                new DictTypeAndData
                {
                    fdDictType = new FdDictType
                    {
                        Id = "11921994044146604",
                        Name = "代码生成方式",
                        Code = "CODE_04",
                        SysFlag = YesNoEnum.Y,
                        OrderNo = 103,
                        Remark = "代码生成方式",
                        Status = StatusEnum.Enable
                    },
                    fdDictData = new List<FdDictData>
                    {
                        new FdDictData{ Id="11921994044146632", DictTypeId="11921994044146604", DictTypeCode="CODE_04", Code="CODE_04_01", Label="下载压缩包", Value="100", ValueType=DictValueType.String, OrderNo=1, Remark="下载压缩包", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146633", DictTypeId="11921994044146604", DictTypeCode="CODE_04", Code="CODE_04_02", Label="下载压缩包 (前端)", Value="111", ValueType=DictValueType.String, OrderNo=2, Remark="下载压缩包 (前端)", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146634", DictTypeId="11921994044146604", DictTypeCode="CODE_04", Code="CODE_04_03", Label="下载压缩包 (后端)", Value="121", ValueType=DictValueType.String, OrderNo=3, Remark="下载压缩包 (后端)", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146635", DictTypeId="11921994044146604", DictTypeCode="CODE_04", Code="CODE_04_04", Label="生成到本项目", Value="200", ValueType=DictValueType.String, OrderNo=4, Remark="生成到本项目", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146636", DictTypeId="11921994044146604", DictTypeCode="CODE_04", Code="CODE_04_05", Label="生成到本项目 (前端)", Value="211", ValueType=DictValueType.String, OrderNo=5, Remark="生成到本项目 (前端)", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146637", DictTypeId="11921994044146604", DictTypeCode="CODE_04", Code="CODE_04_06", Label="生成到本项目 (后端)", Value="221", ValueType=DictValueType.String, OrderNo=6, Remark="生成到本项目 (后端)", Status=StatusEnum.Enable },
                    }
                },
                
                // CODE_05: 代码生成基类
                new DictTypeAndData
                {
                    fdDictType = new FdDictType
                    {
                        Id = "11921994044146605",
                        Name = "代码生成基类",
                        Code = "CODE_05",
                        SysFlag = YesNoEnum.Y,
                        OrderNo = 104,
                        Remark = "代码生成基类",
                        Status = StatusEnum.Enable
                    },
                    fdDictData = new List<FdDictData>
                    {
                        new FdDictData{ Id="11921994044146638", DictTypeId="11921994044146605", DictTypeCode="CODE_05", Code="CODE_05_01", Label="EntityBaseId【基础实体 Id】", Value="EntityBaseId", ValueType=DictValueType.String, OrderNo=1, Remark="【基础实体 Id】", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146639", DictTypeId="11921994044146605", DictTypeCode="CODE_05", Code="CODE_05_02", Label="EntityBase【基础实体】", Value="EntityBase", ValueType=DictValueType.String, OrderNo=1, Remark="【基础实体】", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146640", DictTypeId="11921994044146605", DictTypeCode="CODE_05", Code="CODE_05_03", Label="EntityBaseDel【基础软删除实体】", Value="EntityBaseDel", ValueType=DictValueType.String, OrderNo=1, Remark="【基础软删除实体】", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146641", DictTypeId="11921994044146605", DictTypeCode="CODE_05", Code="CODE_05_04", Label="EntityBaseOrg【机构实体】", Value="EntityBaseOrg", ValueType=DictValueType.String, OrderNo=1, Remark="【机构实体】", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146642", DictTypeId="11921994044146605", DictTypeCode="CODE_05", Code="CODE_05_05", Label="EntityBaseOrgDel【机构软删除实体】", Value="EntityBaseOrgDel", ValueType=DictValueType.String, OrderNo=1, Remark="【机构软删除实体】", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146643", DictTypeId="11921994044146605", DictTypeCode="CODE_05", Code="CODE_05_06", Label="EntityBaseTenantId【租户实体 Id】", Value="EntityBaseTenantId", ValueType=DictValueType.String, OrderNo=1, Remark="【租户实体 Id】", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146644", DictTypeId="11921994044146605", DictTypeCode="CODE_05", Code="CODE_05_07", Label="EntityBaseTenant【租户实体】", Value="EntityBaseTenant", ValueType=DictValueType.String, OrderNo=1, Remark="【租户实体】", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146645", DictTypeId="11921994044146605", DictTypeCode="CODE_05", Code="CODE_05_08", Label="EntityBaseTenantDel【租户软删除实体】", Value="EntityBaseTenantDel", ValueType=DictValueType.String, OrderNo=1, Remark="【租户软删除实体】", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146646", DictTypeId="11921994044146605", DictTypeCode="CODE_05", Code="CODE_05_09", Label="EntityBaseTenantOrg【租户机构实体】", Value="EntityBaseTenantOrg", ValueType=DictValueType.String, OrderNo=1, Remark="【租户机构实体】", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146647", DictTypeId="11921994044146605", DictTypeCode="CODE_05", Code="CODE_05_10", Label="EntityBaseTenantOrgDel【租户机构软删除实体】", Value="EntityBaseTenantOrgDel", ValueType=DictValueType.String, OrderNo=1, Remark="【租户机构软删除实体】", Status=StatusEnum.Enable },
                    }
                },
                
                // CODE_06: 代码生成打印类型
                new DictTypeAndData
                {
                    fdDictType = new FdDictType
                    {
                        Id = "11921994044146606",
                        Name = "代码生成打印类型",
                        Code = "CODE_06",
                        SysFlag = YesNoEnum.Y,
                        OrderNo = 105,
                        Remark = "代码生成打印类型",
                        Status = StatusEnum.Enable
                    },
                    fdDictData = new List<FdDictData>
                    {
                        new FdDictData{ Id="11921994044146648", DictTypeId="11921994044146606", DictTypeCode="CODE_06", Code="CODE_06_01", Label="不需要", Value="off", ValueType=DictValueType.String, OrderNo=100, Remark="不需要打印支持", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146649", DictTypeId="11921994044146606", DictTypeCode="CODE_06", Code="CODE_06_02", Label="绑定打印模版", Value="custom", ValueType=DictValueType.String, OrderNo=101, Remark="绑定打印模版", Status=StatusEnum.Enable },
                    }
                },
                
                // CODE_07: 机构类型
                new DictTypeAndData
                {
                    fdDictType = new FdDictType
                    {
                        Id = "11921994044146607",
                        Name = "机构类型",
                        Code = "CODE_07",
                        SysFlag = YesNoEnum.Y,
                        OrderNo = 201,
                        Remark = "机构类型",
                        Status = StatusEnum.Enable
                    },
                    fdDictData = new List<FdDictData>
                    {
                        new FdDictData{ Id="11921994044146650", DictTypeId="11921994044146607", DictTypeCode="CODE_07", Code="CODE_07_01", Label="集团", Value="101", ValueType=DictValueType.String, OrderNo=100, Remark="集团", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146651", DictTypeId="11921994044146607", DictTypeCode="CODE_07", Code="CODE_07_02", Label="公司", Value="201", ValueType=DictValueType.String, OrderNo=101, Remark="公司", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146652", DictTypeId="11921994044146607", DictTypeCode="CODE_07", Code="CODE_07_03", Label="部门", Value="301", ValueType=DictValueType.String, OrderNo=102, Remark="部门", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146653", DictTypeId="11921994044146607", DictTypeCode="CODE_07", Code="CODE_07_04", Label="区域", Value="401", ValueType=DictValueType.String, OrderNo=103, Remark="区域", Status=StatusEnum.Enable },
                        new FdDictData{ Id="11921994044146654", DictTypeId="11921994044146607", DictTypeCode="CODE_07", Code="CODE_07_05", Label="组", Value="501", ValueType=DictValueType.String, OrderNo=104, Remark="组", Status=StatusEnum.Enable },
                    }
                },
                
                // CODE_08: 邮箱操作业务名称
                new DictTypeAndData
                {
                    fdDictType = new FdDictType
                    {
                        Id = "11921994044146608",
                        Name = "邮箱操作业务名称",
                        Code = "CODE_08",
                        SysFlag = YesNoEnum.Y,
                        OrderNo = 201,
                        Remark = "业务名称",
                        Status = StatusEnum.Enable
                    },
                    fdDictData = new List<FdDictData>
                    {
                        new FdDictData{ Id="11921994044146655", DictTypeId="11921994044146608", DictTypeCode="CODE_08", Code="CODE_08_01", Label="用户注册", Value="UserRegister", ValueType=DictValueType.String, OrderNo=105, Remark="用户注册", Status=StatusEnum.Enable },
                    }
                },
                
                // CODE_09: 系统配置
                new DictTypeAndData
                {
                    fdDictType = new FdDictType
                    {
                        Id = "11921994044146609",
                        Name = "系统配置",
                        Code = "CODE_09",
                        SysFlag = YesNoEnum.Y,
                        OrderNo = 201,
                        Remark = "系统配置",
                        Status = StatusEnum.Enable
                    },
                    fdDictData = new List<FdDictData>
                    {
                        new FdDictData{ Id="12458431967462405", DictTypeId="11921994044146609", DictTypeCode="CODE_09", Code="CODE_09_01", Label="站点域名", Value="http://127.0.0.1:18889", ValueType=DictValueType.String, OrderNo=105, Remark="站点域名", Status=StatusEnum.Enable },
                        new FdDictData{ Id="12458431967462406", DictTypeId="11921994044146609", DictTypeCode="CODE_09", Code="CODE_09_02", Label="站点域名-外网", Value="http://127.0.0.1:18889", ValueType=DictValueType.String, OrderNo=106, Remark="站点外网域名", Status=StatusEnum.Enable },
                    }
                },
                
                // STORAGE_CONFIG: 存储配置
                new DictTypeAndData
                {
                    fdDictType = new FdDictType
                    {
                        Id = "11921994044146711",
                        Name = "存储配置",
                        Code = "STORAGE_CONFIG",
                        SysFlag = YesNoEnum.Y,
                        OrderNo = 203,
                        Remark = "文件存储相关配置",
                        Status = StatusEnum.Enable
                    },
                    fdDictData = new List<FdDictData>
                    {
                        // 控制本地存储返回内网还是外网链接：inner(内网) 或 outer(外网)
                        new FdDictData{ Id="12458431967462408", DictTypeId="11921994044146711", DictTypeCode="STORAGE_CONFIG", Code="LocalStorageLinkType", Label="本地存储链接类型", Value="outer", ValueType=DictValueType.String, OrderNo=1, Remark="本地存储返回链接类型：inner(内网) 或 outer(外网)", Status=StatusEnum.Enable },
                    }
                },
                
                // PASSWORD_CONFIG: 密码加密配置
                new DictTypeAndData
                {
                    fdDictType = new FdDictType
                    {
                        Id = "11921994044146610",
                        Name = "密码加密配置",
                        Code = "PASSWORD_CONFIG",
                        SysFlag = YesNoEnum.Y,
                        OrderNo = 202,
                        Remark = "密码加密相关配置",
                        Status = StatusEnum.Enable
                    },
                    fdDictData = new List<FdDictData>
                    {
                        // 密码加密类型：Irreversible(不可逆，默认) 或 Reversible(可逆)
                        new FdDictData{ Id="11921994044146656", DictTypeId="11921994044146610", DictTypeCode="PASSWORD_CONFIG", Code="PasswordHashType", Label="密码加密类型", Value="Irreversible", ValueType=DictValueType.String, OrderNo=1, Remark="密码加密类型：Irreversible(不可逆，推荐) 或 Reversible(可逆)", Status=StatusEnum.Enable },
                        // 密码加密密钥（仅在可逆加密时使用）
                        // 注意：生产环境应该重新生成更安全的密钥，并使用安全的密钥管理方式
                        new FdDictData{ Id="11921994044146657", DictTypeId="11921994044146610", DictTypeCode="PASSWORD_CONFIG", Code="PasswordEncryptionKey", Label="密码加密密钥", Value="APtTP1MbsRf2U6WzDzG6vd6qLBIZGPTI5yMwsf94pkA=", ValueType=DictValueType.String, OrderNo=2, Remark="密码加密密钥（Base64格式，256位AES密钥），仅在可逆加密时使用。生产环境请使用 CryptographyUtils.GenerateAESKey() 重新生成", Status=StatusEnum.Enable },
                        // 默认密码（用于用户初始化和批量导入）
                        new FdDictData{ Id="11921994044146658", DictTypeId="11921994044146610", DictTypeCode="PASSWORD_CONFIG", Code="DefaultUserPassword", Label="默认用户密码", Value="123456", ValueType=DictValueType.String, OrderNo=3, Remark="新用户初始化或批量导入时的默认密码。系统会自动对此密码进行哈希处理后存储。建议首次登录后立即修改", Status=StatusEnum.Enable },
                    }
                },
                //用户相关配置
                new DictTypeAndData
                {
                    fdDictType = new FdDictType
                    {
                        Id = "12725437239198726",
                        Name = "用户相关配置",
                        Code = "CODE_10",
                        SysFlag = YesNoEnum.Y,
                        OrderNo = 201,
                        Remark = "用户相关配置",
                        Status = StatusEnum.Enable
                    },
                    fdDictData = new List<FdDictData>
                    {
                        new FdDictData{ Id="12725437239133189", DictTypeId="12725437239198726", DictTypeCode="CODE_10", Code="CODE_10_01", Label="账号长度最小限制", Value="6", ValueType=DictValueType.String, OrderNo=105, Remark="最短用户名长度", Status=StatusEnum.Enable },
                        new FdDictData{ Id="12725437239198725", DictTypeId="12725437239198726", DictTypeCode="CODE_10", Code="CODE_10_02", Label="账号长度最大限制", Value="15", ValueType=DictValueType.String, OrderNo=106, Remark="最长用户名长度", Status=StatusEnum.Enable },
                        new FdDictData{ Id="12721271026222085", DictTypeId="12725437239198726", DictTypeCode="CODE_10", Code="CODE_10_03", Label="密码强度正则", Value="^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z0-9]).{10,20}$", ValueType=DictValueType.String, OrderNo=106, Remark="必须包含大小写字母、数字、特殊符号，长度10-20位", Status=StatusEnum.Enable },
                        new FdDictData{ Id="12721215122441221", DictTypeId="12725437239198726", DictTypeCode="CODE_10", Code="CODE_10_04", Label="用户名正则", Value="^[a-z][a-z0-9]{5,14}$", ValueType=DictValueType.String, OrderNo=106, Remark="必须以字母开头，仅包含小写字母或数字，总长度6-15位", Status=StatusEnum.Enable },
                    }
                },
            };

            // 调用服务保存字典数据
            await _dictInitializerService.SaveDictDataAsync(dictTypeAndDataList);
            
            Console.WriteLine("[FdDictInitializer] 字典数据初始化完成");
            Console.WriteLine("[FdDictInitializer] 已初始化 PASSWORD_CONFIG.DefaultUserPassword = '1234567'");
        }
    }
}
