using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace Fastdotnet.Core.Attributes
{
    // 1.一个专门处理 UTC 的 Converter
    public class IsoDateTimeConverterUtc : IsoDateTimeConverter
    {
        public IsoDateTimeConverterUtc()
        {
            // 强制指定为 UTC
            DateTimeStyles = System.Globalization.DateTimeStyles.AssumeUniversal;
            // 格式化字符串，末尾加上 'Z' 表示 UTC
            DateTimeFormat = "yyyy-MM-dd HH:mm:ssZ";
        }
    }
}
