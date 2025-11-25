using Fastdotnet.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginA.Dto
{
    /// <summary>
    /// 自定义脱敏示例DTO
    /// </summary>
    public class PluginACustomTestDto
    {
        public string Id { get; set; }
        
        public string Name { get; set; }
        
        /// <summary>
        /// 使用自定义脱敏规则的地址字段
        /// 脱敏规则：保留前6位和后4位，中间用****替代
        /// </summary>
        [SensitiveData(SensitiveDataType.Custom, 
                      CustomPattern = @"(\d{6})\d+(\d{4})", 
                      CustomReplacement = "$1****$2")]
        public string Address { get; set; }
        
        /// <summary>
        /// 使用自定义脱敏规则的社会安全号码
        /// 脱敏规则：保留前3位和后2位，中间用*****替代
        /// </summary>
        [SensitiveData(SensitiveDataType.Custom,
                      CustomPattern = @"(\d{3})\d+(\d{2})",
                      CustomReplacement = "$1*****$2")]
        public string SSN { get; set; }
        
        public DateTime CreateTime { get; set; }
    }
}