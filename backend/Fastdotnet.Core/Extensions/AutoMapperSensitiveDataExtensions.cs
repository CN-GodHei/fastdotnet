using AutoMapper;
using Fastdotnet.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Fastdotnet.Core.Extensions
{
    public static class AutoMapperSensitiveDataExtensions
    {
        public static IMappingExpression<TSource, TDestination>
            MaskSensitiveData<TSource, TDestination>(
                this IMappingExpression<TSource, TDestination> mapping)
        {
            // 遍历目标类型的所有属性，查找标记了敏感数据的属性
            var destinationType = typeof(TDestination);
            var sensitiveProperties = destinationType.GetProperties()
                .Where(p => p.GetCustomAttribute<SensitiveDataAttribute>() != null);

            foreach (var property in sensitiveProperties)
            {
                var attr = property.GetCustomAttribute<SensitiveDataAttribute>();
                mapping.ForMember(property.Name, opt =>
                    opt.MapFrom((src, dest) => 
                    {
                        try
                        {
                            return MaskValue(src, property, attr);
                        }
                        catch
                        {
                            // 如果脱敏过程中出现任何异常，返回原始值
                            return src != null ? property.GetValue(src) : null;
                        }
                    }));
            }

            return mapping;
        }

        // 实现MaskValue方法
        private static object MaskValue<TSource>(TSource src, PropertyInfo property, SensitiveDataAttribute attr)
        {
            if (src == null) return null;
            
            try
            {
                // 确保src对象包含同名属性
                var srcProperty = typeof(TSource).GetProperty(property.Name);
                if (srcProperty == null) 
                {
                    // 如果源对象没有同名属性，尝试直接获取值
                    return property.GetValue(src);
                }
                
                var value = srcProperty.GetValue(src);
                if (value == null) return null;

                // 根据不同的数据类型进行脱敏处理
                return attr.DataType switch
                {
                    SensitiveDataType.Phone => MaskPhone(value.ToString(), attr),
                    SensitiveDataType.Email => MaskEmail(value.ToString(), attr),
                    SensitiveDataType.IdCard => MaskIdCard(value.ToString(), attr),
                    SensitiveDataType.BankCard => MaskBankCard(value.ToString(), attr),
                    SensitiveDataType.Name => MaskName(value.ToString(), attr),
                    SensitiveDataType.Custom => MaskCustom(value.ToString(), attr),
                    _ => value
                };
            }
            catch
            {
                // 如果在获取值或脱敏过程中出现异常，返回原始值
                return property.GetValue(src);
            }
        }

        // 手机号脱敏 13812345678 -> 138****5678
        private static string MaskPhone(string phone, SensitiveDataAttribute attribute)
        {
            // 如果设置了自定义参数，则使用自定义规则
            if (attribute.PrefixKeep > 0 || attribute.SuffixKeep > 0)
            {
                return MaskWithParams(phone, attribute.PrefixKeep, attribute.SuffixKeep, attribute.MaskChar, attribute.MaskLength);
            }
            
            // 否则使用默认规则
            if (string.IsNullOrEmpty(phone) || phone.Length < 11)
                return phone;

            return Regex.Replace(phone, @"(\d{3})\d{4}(\d{4})", "$1****$2");
        }

        // 邮箱脱敏 example@domain.com -> ex***le@domain.com
        private static string MaskEmail(string email, SensitiveDataAttribute attribute)
        {
            // 如果设置了自定义参数，则使用自定义规则
            if (attribute.PrefixKeep > 0 || attribute.SuffixKeep > 0)
            {
                var emailParts = email.Split('@');
                if (emailParts.Length != 2)
                    return email;

                var csmusername = emailParts[0];
                var csmdomain = emailParts[1];
                var csmmaskedUsername = MaskWithParams(csmusername, attribute.PrefixKeep, attribute.SuffixKeep, attribute.MaskChar, attribute.MaskLength);
                return csmmaskedUsername + "@" + csmdomain;
            }
            
            // 否则使用默认规则
            if (string.IsNullOrEmpty(email) || !email.Contains("@"))
                return email;

            var parts = email.Split('@');
            var username = parts[0];
            var domain = parts[1];

            if (username.Length <= 2)
                return email;

            var maskedUsername = username.Substring(0, 1) + "***" + username.Substring(username.Length - 1);
            return maskedUsername + "@" + domain;
        }

        // 身份证脱敏 110101199001011234 -> 110101********1234
        private static string MaskIdCard(string idCard, SensitiveDataAttribute attribute)
        {
            // 如果设置了自定义参数，则使用自定义规则
            if (attribute.PrefixKeep > 0 || attribute.SuffixKeep > 0)
            {
                return MaskWithParams(idCard, attribute.PrefixKeep, attribute.SuffixKeep, attribute.MaskChar, attribute.MaskLength);
            }
            
            // 否则使用默认规则
            if (string.IsNullOrEmpty(idCard) || idCard.Length < 14)
                return idCard;

            return idCard.Substring(0, 6) + "********" + idCard.Substring(idCard.Length - 4);
        }

        // 银行卡脱敏 6222021234567890 -> 622202******7890
        private static string MaskBankCard(string bankCard, SensitiveDataAttribute attribute)
        {
            // 如果设置了自定义参数，则使用自定义规则
            if (attribute.PrefixKeep > 0 || attribute.SuffixKeep > 0)
            {
                return MaskWithParams(bankCard, attribute.PrefixKeep, attribute.SuffixKeep, attribute.MaskChar, attribute.MaskLength);
            }
            
            // 否则使用默认规则
            if (string.IsNullOrEmpty(bankCard) || bankCard.Length < 10)
                return bankCard;

            return bankCard.Substring(0, 6) + "******" + bankCard.Substring(bankCard.Length - 4);
        }

        // 姓名脱敏 张三 -> 张*
        private static string MaskName(string name, SensitiveDataAttribute attribute)
        {
            // 如果设置了自定义参数，则使用自定义规则
            if (attribute.PrefixKeep > 0 || attribute.SuffixKeep > 0)
            {
                return MaskWithParams(name, attribute.PrefixKeep, attribute.SuffixKeep, attribute.MaskChar, attribute.MaskLength);
            }
            
            // 否则使用默认规则
            if (string.IsNullOrEmpty(name) || name.Length < 2)
                return name;

            if (name.Length == 2)
                return name.Substring(0, 1) + "*";

            var first = name.Substring(0, 1);
            var last = name.Substring(name.Length - 1, 1);
            var middle = new string('*', name.Length - 2);
            return first + middle + last;
        }

        // 自定义脱敏规则
        private static string MaskCustom(string value, SensitiveDataAttribute attribute)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(attribute.CustomPattern))
                return value;

            try
            {
                return Regex.Replace(value, attribute.CustomPattern, attribute.CustomReplacement ?? "***");
            }
            catch
            {
                // 正则表达式无效时，返回原始值
                return value;
            }
        }

        // 通用脱敏方法（支持自定义参数）
        private static string MaskWithParams(string value, int prefixKeep, int suffixKeep, char maskChar, int maskLength)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            // 如果没有设置参数，则使用默认值
            prefixKeep = prefixKeep > 0 ? prefixKeep : 0;
            suffixKeep = suffixKeep > 0 ? suffixKeep : 0;
            
            // 确保不会越界
            if (prefixKeep + suffixKeep >= value.Length)
            {
                // 如果保留的字符数大于等于总长度，则全部用脱敏字符替代
                maskLength = maskLength > 0 ? maskLength : Math.Max(1, value.Length);
                return new string(maskChar, maskLength);
            }

            var prefix = prefixKeep > 0 ? value.Substring(0, prefixKeep) : "";
            var suffix = suffixKeep > 0 ? value.Substring(value.Length - suffixKeep) : "";
            
            // 计算中间脱敏部分的长度
            var maskPartLength = value.Length - prefixKeep - suffixKeep;
            maskLength = maskLength > 0 ? maskLength : maskPartLength;
            
            var maskPart = new string(maskChar, maskLength);
            
            return prefix + maskPart + suffix;
        }
    }
}