using Fastdotnet.Core.Attributes;
using Mapster;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Fastdotnet.Core.Extensions
{
    /// <summary>
    /// Mapster 敏感数据脱敏扩展方法
    /// </summary>
    public static class MapsterSensitiveDataExtensions
    {
        /// <summary>
        /// 为 Mapster 映射配置启用敏感数据脱敏
        /// </summary>
        public static TypeAdapterSetter<TSource, TDestination> 
            MaskSensitiveData<TSource, TDestination>(
                this TypeAdapterSetter<TSource, TDestination> setter)
        {
            // 遍历目标类型的所有属性，查找标记了敏感数据的属性
            var destinationType = typeof(TDestination);
            var sensitiveProperties = destinationType.GetProperties()
                .Where(p => p.GetCustomAttribute<SensitiveDataAttribute>() != null)
                .ToList();

            if (!sensitiveProperties.Any())
                return setter;

            // 使用 AfterMapping 在映射完成后进行脱敏处理
            setter.AfterMapping((src, dest) =>
            {
                if (dest == null) return;

                foreach (var property in sensitiveProperties)
                {
                    var attr = property.GetCustomAttribute<SensitiveDataAttribute>();
                    var value = property.GetValue(dest);
                    
                    if (value == null) continue;

                    var maskedValue = attr.DataType switch
                    {
                        SensitiveDataType.Phone => MaskPhone(value.ToString(), attr),
                        SensitiveDataType.Email => MaskEmail(value.ToString(), attr),
                        SensitiveDataType.IdCard => MaskIdCard(value.ToString(), attr),
                        SensitiveDataType.BankCard => MaskBankCard(value.ToString(), attr),
                        SensitiveDataType.Name => MaskName(value.ToString(), attr),
                        SensitiveDataType.Custom => MaskCustom(value.ToString(), attr),
                        _ => value
                    };

                    property.SetValue(dest, maskedValue);
                }
            });

            return setter;
        }

        /// <summary>
        /// 手机号脱敏 13812345678 -> 138****5678
        /// </summary>
        private static string MaskPhone(string phone, SensitiveDataAttribute attribute)
        {
            if (attribute.PrefixKeep > 0 || attribute.SuffixKeep > 0)
            {
                return MaskWithParams(phone, attribute.PrefixKeep, attribute.SuffixKeep, 
                    attribute.MaskChar, attribute.MaskLength);
            }
            
            if (string.IsNullOrEmpty(phone) || phone.Length < 11)
                return phone;

            return Regex.Replace(phone, @"(\d{3})\d{4}(\d{4})", "$1****$2");
        }

        // 邮箱脱敏 example@domain.com -> ex***le@domain.com
        private static string MaskEmail(string email, SensitiveDataAttribute attribute)
        {
            if (attribute.PrefixKeep > 0 || attribute.SuffixKeep > 0)
            {
                var emailParts = email.Split('@');
                if (emailParts.Length != 2)
                    return email;

                var csmusername = emailParts[0];
                var csmdomain = emailParts[1];
                var csmmaskedUsername = MaskWithParams(csmusername, attribute.PrefixKeep, 
                    attribute.SuffixKeep, attribute.MaskChar, attribute.MaskLength);
                return csmmaskedUsername + "@" + csmdomain;
            }
            
            if (string.IsNullOrEmpty(email) || !email.Contains("@"))
                return email;

            var parts = email.Split('@');
            var username = parts[0];
            var domain = parts[1];

            if (username.Length <= 2)
                return email;

            var maskedUsername = username.Substring(0, 1) + "***" + 
                username.Substring(username.Length - 1);
            return maskedUsername + "@" + domain;
        }

        // 身份证脱敏 110101199001011234 -> 110101********1234
        private static string MaskIdCard(string idCard, SensitiveDataAttribute attribute)
        {
            if (attribute.PrefixKeep > 0 || attribute.SuffixKeep > 0)
            {
                return MaskWithParams(idCard, attribute.PrefixKeep, attribute.SuffixKeep, 
                    attribute.MaskChar, attribute.MaskLength);
            }
            
            if (string.IsNullOrEmpty(idCard) || idCard.Length < 14)
                return idCard;

            return idCard.Substring(0, 6) + "********" + idCard.Substring(idCard.Length - 4);
        }

        // 银行卡脱敏 6222021234567890 -> 622202******7890
        private static string MaskBankCard(string bankCard, SensitiveDataAttribute attribute)
        {
            if (attribute.PrefixKeep > 0 || attribute.SuffixKeep > 0)
            {
                return MaskWithParams(bankCard, attribute.PrefixKeep, attribute.SuffixKeep, 
                    attribute.MaskChar, attribute.MaskLength);
            }
            
            if (string.IsNullOrEmpty(bankCard) || bankCard.Length < 10)
                return bankCard;

            return bankCard.Substring(0, 6) + "******" + 
                bankCard.Substring(bankCard.Length - 4);
        }

        // 姓名脱敏 张三 -> 张*
        private static string MaskName(string name, SensitiveDataAttribute attribute)
        {
            if (attribute.PrefixKeep > 0 || attribute.SuffixKeep > 0)
            {
                return MaskWithParams(name, attribute.PrefixKeep, attribute.SuffixKeep, 
                    attribute.MaskChar, attribute.MaskLength);
            }
            
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
                return Regex.Replace(value, attribute.CustomPattern, 
                    attribute.CustomReplacement ?? "***");
            }
            catch
            {
                return value;
            }
        }

        // 通用脱敏方法（支持自定义参数）
        private static string MaskWithParams(string value, int prefixKeep, int suffixKeep, 
            char maskChar, int maskLength)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            prefixKeep = prefixKeep > 0 ? prefixKeep : 0;
            suffixKeep = suffixKeep > 0 ? suffixKeep : 0;
            
            if (prefixKeep + suffixKeep >= value.Length)
            {
                maskLength = maskLength > 0 ? maskLength : Math.Max(1, value.Length);
                return new string(maskChar, maskLength);
            }

            var prefix = prefixKeep > 0 ? value.Substring(0, prefixKeep) : "";
            var suffix = suffixKeep > 0 ? value.Substring(value.Length - suffixKeep) : "";
            
            var maskPartLength = value.Length - prefixKeep - suffixKeep;
            maskLength = maskLength > 0 ? maskLength : maskPartLength;
            
            var maskPart = new string(maskChar, maskLength);
            
            return prefix + maskPart + suffix;
        }
    }
}
