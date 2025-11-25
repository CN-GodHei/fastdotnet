using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fastdotnet.Core.Attributes
{
    // 定义脱敏属性标记
    [AttributeUsage(AttributeTargets.Property)]
    public class SensitiveDataAttribute : Attribute
    {
        public SensitiveDataType DataType { get; }
        
        // 自定义正则表达式（仅在DataType为Custom时使用）
        public string CustomPattern { get; set; }
        
        // 自定义替换字符串（仅在DataType为Custom时使用）
        public string CustomReplacement { get; set; }
        
        // 保留前缀字符数
        public int PrefixKeep { get; set; } = -1;
        
        // 保留后缀字符数
        public int SuffixKeep { get; set; } = -1;
        
        // 脱敏字符
        public char MaskChar { get; set; } = '*';
        
        // 脱敏字符长度（仅在特定情况下使用）
        public int MaskLength { get; set; } = -1;

        public SensitiveDataAttribute(SensitiveDataType dataType)
        {
            DataType = dataType;
        }
    }

    public enum SensitiveDataType
    {
        Phone,
        Email,
        IdCard,
        BankCard,
        Name,
        Custom  // 自定义脱敏规则
    }
}