using System.ComponentModel;
using Fastdotnet.Core.Entities.Sys;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fastdotnet.Core.Utils
{
    /// <summary>
    /// 字典值类型转换工具类
    /// 根据字典数据的 ValueType 字段自动转换为对应的.NET 类型
    /// </summary>
    public static class DictValueConverter
    {
        /// <summary>
        /// 将字典值转换为指定的.NET 类型
        /// </summary>
        /// <param name="dictData">字典数据</param>
        /// <returns>转换后的对象</returns>
        public static object? ConvertToDotNetType(FdDictData dictData)
        {
            if (dictData == null || string.IsNullOrEmpty(dictData.Value))
            {
                return null;
            }

            return ConvertByValueType(dictData.Value, dictData.ValueType);
        }

        /// <summary>
        /// 根据值类型枚举转换为对应的.NET 类型（主要方法）
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="valueType">值类型枚举</param>
        /// <returns>转换后的对象</returns>
        public static object? ConvertByValueType(string value, DictValueType valueType)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            return valueType switch
            {
                DictValueType.Int => ConvertToInt(value),
                DictValueType.Long => ConvertToLong(value),
                DictValueType.Double => ConvertToDouble(value),
                DictValueType.Decimal => ConvertToDecimal(value),
                DictValueType.Boolean => ConvertToBoolean(value),
                DictValueType.DateTime => ConvertToDateTime(value),
                DictValueType.Json => ConvertToJson(value),
                DictValueType.JsonArray => ConvertToJsonArray(value),
                DictValueType.String or _ => value
            };
        }

        /// <summary>
        /// 根据值类型字符串转换为对应的.NET 类型（兼容方法）
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="valueType">值类型字符串</param>
        /// <returns>转换后的对象</returns>
        public static object? ConvertByValueType(string value, string valueType)
        {
            if (string.IsNullOrEmpty(valueType))
            {
                return value;
            }

            // 尝试解析字符串为枚举
            if (global::System.Enum.TryParse<DictValueType>(valueType, true, out var parsedType))
            {
                return ConvertByValueType(value, parsedType);
            }

            // 兼容旧版本的字符串格式
            return valueType?.ToLower() switch
            {
                "int" or "integer" => ConvertToInt(value),
                "long" => ConvertToLong(value),
                "double" => ConvertToDouble(value),
                "decimal" or "money" => ConvertToDecimal(value),
                "boolean" or "bool" => ConvertToBoolean(value),
                "datetime" or "date" => ConvertToDateTime(value),
                "json" => ConvertToJson(value),
                "json_array" => ConvertToJsonArray(value),
                "string" or _ => value
            };
        }

        /// <summary>
        /// 转换为整数
        /// </summary>
        private static int ConvertToInt(string value)
        {
            return int.TryParse(value, out var result) ? result : 0;
        }

        /// <summary>
        /// 转换为长整型
        /// </summary>
        private static long ConvertToLong(string value)
        {
            return long.TryParse(value, out var result) ? result : 0L;
        }

        /// <summary>
        /// 转换为浮点型
        /// </summary>
        private static double ConvertToDouble(string value)
        {
            return double.TryParse(value, out var result) ? result : 0.0;
        }

        /// <summary>
        /// 转换为金额（Decimal）
        /// </summary>
        private static decimal ConvertToDecimal(string value)
        {
            return decimal.TryParse(value, out var result) ? result : 0m;
        }

        /// <summary>
        /// 转换为布尔值
        /// </summary>
        private static bool ConvertToBoolean(string value)
        {
            return value.ToLower() is "true" or "1" or "yes" or "y";
        }

        /// <summary>
        /// 转换为日期时间
        /// </summary>
        private static DateTime ConvertToDateTime(string value)
        {
            return DateTime.TryParse(value, out var result) ? result : DateTime.MinValue;
        }

        /// <summary>
        /// 转换为 JSON 对象
        /// </summary>
        private static JObject? ConvertToJson(string value)
        {
            try
            {
                return JObject.Parse(value);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 转换为 JSON 数组
        /// </summary>
        private static JArray? ConvertToJsonArray(string value)
        {
            try
            {
                return JArray.Parse(value);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 转换为强类型泛型
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="dictData">字典数据</param>
        /// <returns>转换后的强类型对象</returns>
        public static T? ConvertToType<T>(FdDictData dictData)
        {
            var value = ConvertToDotNetType(dictData);
            if (value == null)
            {
                return default;
            }

            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// 获取值类型的枚举描述
        /// </summary>
        /// <param name="valueType">值类型枚举</param>
        /// <returns>类型描述</returns>
        public static string GetValueTypeDescription(DictValueType valueType)
        {
            return valueType switch
            {
                DictValueType.Int => "整数",
                DictValueType.Long => "长整型",
                DictValueType.Double => "浮点数",
                DictValueType.Decimal => "金额",
                DictValueType.Boolean => "布尔",
                DictValueType.DateTime => "日期时间",
                DictValueType.Json => "JSON 对象",
                DictValueType.JsonArray => "JSON 数组",
                DictValueType.String or _ => "字符串"
            };
        }

        /// <summary>
        /// 获取值类型的字符串表示（用于序列化到数据库）
        /// </summary>
        /// <param name="valueType">值类型枚举</param>
        /// <returns>小写英文字符串</returns>
        public static string GetValueTypeString(DictValueType valueType)
        {
            return valueType.ToString().ToLower();
        }

        /// <summary>
        /// 验证值是否与类型匹配
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="valueType">值类型枚举</param>
        /// <returns>是否有效</returns>
        public static bool ValidateValueType(string value, DictValueType valueType)
        {
            if (string.IsNullOrEmpty(value))
            {
                return true; // 空值认为有效
            }

            return valueType switch
            {
                DictValueType.Int => int.TryParse(value, out _),
                DictValueType.Long => long.TryParse(value, out _),
                DictValueType.Double => double.TryParse(value, out _),
                DictValueType.Decimal => decimal.TryParse(value, out _),
                DictValueType.Boolean => bool.TryParse(value, out _),
                DictValueType.DateTime => DateTime.TryParse(value, out _),
                DictValueType.Json => IsValidJson(value),
                DictValueType.JsonArray => IsValidJsonArray(value),
                DictValueType.String or _ => true
            };
        }

        /// <summary>
        /// 验证值是否与类型字符串匹配（兼容方法）
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="valueType">值类型字符串</param>
        /// <returns>是否有效</returns>
        public static bool ValidateValueType(string value, string valueType)
        {
            if (string.IsNullOrEmpty(valueType))
            {
                return true;
            }

            if (global::System.Enum.TryParse<DictValueType>(valueType, true, out var parsedType))
            {
                return ValidateValueType(value, parsedType);
            }

            return valueType?.ToLower() switch
            {
                "int" or "integer" => int.TryParse(value, out _),
                "long" => long.TryParse(value, out _),
                "double" => double.TryParse(value, out _),
                "decimal" or "money" => decimal.TryParse(value, out _),
                "boolean" or "bool" => bool.TryParse(value, out _),
                "datetime" or "date" => DateTime.TryParse(value, out _),
                "json" => IsValidJson(value),
                "json_array" => IsValidJsonArray(value),
                "string" or _ => true
            };
        }

        /// <summary>
        /// 是否是有效的 JSON 对象
        /// </summary>
        private static bool IsValidJson(string value)
        {
            try
            {
                JObject.Parse(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 是否是有效的 JSON 数组
        /// </summary>
        private static bool IsValidJsonArray(string value)
        {
            try
            {
                JArray.Parse(value);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
