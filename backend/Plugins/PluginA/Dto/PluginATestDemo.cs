using Fastdotnet.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginA.Dto
{
    /// <summary>
    /// 新增传输对象
    /// </summary>
    public class PluginATestDemoCreateDto
    {
        /// <summary>
        /// 名称 - 必填字段
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 测试数值
        /// </summary>
        public int TestValue { get; set; }

        /// <summary>
        /// 是否启用 - 默认为true
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// 创建人手机号
        /// </summary>
        public string Creator { get; set; }
    }

    /// <summary>
    /// 修改传输对象
    /// </summary>
    public class PluginATestDemoUpdateDto
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 测试数值
        /// </summary>
        public int TestValue { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 创建人手机号
        /// </summary>
        public string Creator { get; set; }
    }

    /// <summary>
    /// 输出传输对象 - 包含完整信息和脱敏规则
    /// </summary>
    public class PluginATestDemoDto
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 测试数值
        /// </summary>
        public int TestValue { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 创建人手机号 - 手机号脱敏：138****8888
        /// </summary>
        [SensitiveData(SensitiveDataType.Phone)]
        public string Creator { get; set; }

        /// <summary>
        /// 身份证号码 - 身份证脱敏：123456**********12X
        /// </summary>
        [SensitiveData(SensitiveDataType.IdCard, PrefixKeep = 6, SuffixKeep = 3, MaskChar = '*', MaskLength = 10)]
        public string IdCard { get; set; }

        /// <summary>
        /// 电子邮箱 - 邮箱脱敏：z***@example.com
        /// </summary>
        [SensitiveData(SensitiveDataType.Email, PrefixKeep = 1, SuffixKeep = 10)]
        public string Email { get; set; }

        /// <summary>
        /// 银行卡号 - 自定义脱敏：保留前6位和后4位
        /// </summary>
        [SensitiveData(SensitiveDataType.BankCard, PrefixKeep = 6, SuffixKeep = 4)]
        public string BankCardNo { get; set; }

        /// <summary>
        /// IP地址 - 网络安全信息脱敏
        /// </summary>
        [SensitiveData(SensitiveDataType.Custom, CustomPattern = @"(\d{1,3}\.\d{1,3}\.)\d{1,3}\.\d{1,3}", CustomReplacement = "$1***.***")]
        public string IpAddress { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }
    }
}