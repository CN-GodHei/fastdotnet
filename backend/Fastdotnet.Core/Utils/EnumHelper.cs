using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Fastdotnet.Core.Utils
{

    /// <summary>
    /// 枚举工具类
    /// </summary>
    public static class EnumHelper
    {
        private static readonly ConcurrentDictionary<System.Enum, string> _descriptionCache = new();
        private static readonly ConcurrentDictionary<(Type enumType, string description), System.Enum> _enumFromDescCache = new();
        
        /// <summary>
        /// 获取枚举的Description特性描述
        /// </summary>
        /// <param name="value">枚举值</param>
        /// <returns>描述信息，如果未找到则返回枚举名称</returns>
        public static string GetDescription(System.Enum value)
        {
            if (value == null) return string.Empty;

            if (_descriptionCache.TryGetValue(value, out var desc))
                return desc;

            var field = value.GetType().GetField(value.ToString());
            desc = field?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? value.ToString();

            _descriptionCache.TryAdd(value, desc);
            return desc;
        }

        /// <summary>
        /// 根据描述获取对应的枚举值
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="description">描述</param>
        /// <param name="result">获取到的枚举值</param>
        /// <returns>如果找到对应枚举值返回true，否则返回false</returns>
        public static bool TryGetEnumFromDescription<T>(string description, out T result)
            where T : struct, System.Enum
        {
            result = default;
            if (string.IsNullOrEmpty(description)) return false;

            var key = (typeof(T), description);
            if (_enumFromDescCache.TryGetValue(key, out var cachedResult))
            {
                result = (T)cachedResult;
                return true;
            }

            var type = typeof(T);
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var attr = field.GetCustomAttribute<DescriptionAttribute>();
                if (attr?.Description == description)
                {
                    result = (T)field.GetValue(null);
                    _enumFromDescCache.TryAdd(key, result);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 将枚举转换为字典集合
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <returns>包含枚举值和描述的字典</returns>
        public static Dictionary<int, string> ToDescriptionDictionary<T>() where T : System.Enum
        {
            var dictionary = new Dictionary<int, string>();
            var type = typeof(T);

            foreach (var value in System.Enum.GetValues(type))
            {
                var field = type.GetField(value.ToString());
                var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
                dictionary.Add(Convert.ToInt32(value), attribute?.Description ?? value.ToString());
            }

            return dictionary;
        }

        /// <summary>
        /// 获取枚举类型的所有成员及其描述
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <returns>枚举成员和描述的列表</returns>
        public static List<(T Value, string Description)> GetAllMembers<T>() where T : System.Enum
        {
            return System.Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(v => (v, GetDescription(v)))
                .ToList();
        }

        /// <summary>
        /// 比较传入的值与枚举值是否相等
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="value">要比较的值（可以是数字或字符串）</param>
        /// <param name="enumValue">枚举值</param>
        /// <returns>如果相等返回true，否则返回false</returns>
        public static bool IsEqual<T>(object value, T enumValue) where T : struct, System.Enum
        {
            if (value == null) return false;

            return value switch
            {
                string strValue => System.Enum.TryParse<T>(strValue, true, out var parsedEnum) &&
                                  EqualityComparer<T>.Default.Equals(parsedEnum, enumValue),
                _ => TryConvertAndCompare(value, enumValue)
            };
        }

        /// <summary>
        /// 尝试转换并比较对象值与枚举值
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="value">要比较的值</param>
        /// <param name="enumValue">枚举值</param>
        /// <returns>如果相等返回true，否则返回false</returns>
        private static bool TryConvertAndCompare<T>(object value, T enumValue) where T : struct, System.Enum
        {
            try
            {
                var enumUnderlyingValue = Convert.ChangeType(enumValue, System.Enum.GetUnderlyingType(typeof(T)));
                var compareValue = Convert.ChangeType(value, System.Enum.GetUnderlyingType(typeof(T)));
                return enumUnderlyingValue.Equals(compareValue);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 将指定的值转换为枚举类型
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="value">要转换的值（可以是数字或字符串）</param>
        /// <returns>转换后的枚举值，如果转换失败则返回默认值</returns>
        public static T ParseEnum<T>(object value) where T : struct, System.Enum
        {
            if (value == null) return default;

            return value switch
            {
                string strValue => System.Enum.TryParse<T>(strValue, true, out var result) ? result : default,
                _ => HandleNumericValue<T>(value)
            };
        }

        /// <summary>
        /// 处理数值类型的值并尝试转换为枚举
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="value">要转换的数值</param>
        /// <returns>转换后的枚举值，如果转换失败则返回默认值</returns>
        private static T HandleNumericValue<T>(object value) where T : struct, System.Enum
        {
            try
            {
                if (System.Enum.IsDefined(typeof(T), value))
                {
                    return (T)System.Enum.ToObject(typeof(T), value);
                }
            }
            catch
            {
                // 忽略异常，返回默认值
            }

            return default;
        }
    }
}