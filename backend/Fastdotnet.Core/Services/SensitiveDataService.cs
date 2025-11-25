using Fastdotnet.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Fastdotnet.Core.Services
{
    /// <summary>
    /// 敏感数据脱敏服务
    /// </summary>
    public class SensitiveDataService
    {
        /// <summary>
        /// 对标记了SensitiveData特性的对象属性进行脱敏处理
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">需要脱敏的对象</param>
        /// <returns>脱敏后的对象</returns>
        public T MaskSensitiveData<T>(T obj) where T : class
        {
            if (obj == null) return null;

            var properties = typeof(T).GetProperties()
                .Where(p => p.GetCustomAttribute<SensitiveDataAttribute>() != null && 
                            p.CanWrite);

            foreach (var property in properties)
            {
                var attr = property.GetCustomAttribute<SensitiveDataAttribute>();
                var value = property.GetValue(obj);

                if (value is string stringValue)
                {
                    var maskedValue = MaskString(stringValue, attr);
                    property.SetValue(obj, maskedValue);
                }
            }

            return obj;
        }

        /// <summary>
        /// 对字符串进行脱敏处理
        /// </summary>
        /// <param name="value">原始字符串</param>
        /// <param name="attribute">敏感数据特性</param>
        /// <returns>脱敏后的字符串</returns>
        public string MaskString(string value, SensitiveDataAttribute attribute)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return attribute.DataType switch
            {
                SensitiveDataType.Phone => MaskPhone(value, attribute),
                SensitiveDataType.Email => MaskEmail(value, attribute),
                SensitiveDataType.IdCard => MaskIdCard(value, attribute),
                SensitiveDataType.BankCard => MaskBankCard(value, attribute),
                SensitiveDataType.Name => MaskName(value, attribute),
                SensitiveDataType.Custom => MaskCustom(value, attribute),
                _ => value
            };
        }

        /// <summary>
        /// 对字符串进行脱敏处理（简化版本）
        /// </summary>
        /// <param name="value">原始字符串</param>
        /// <param name="dataType">数据类型</param>
        /// <returns>脱敏后的字符串</returns>
        public string MaskString(string value, SensitiveDataType dataType)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return dataType switch
            {
                SensitiveDataType.Phone => MaskPhone(value),
                SensitiveDataType.Email => MaskEmail(value),
                SensitiveDataType.IdCard => MaskIdCard(value),
                SensitiveDataType.BankCard => MaskBankCard(value),
                SensitiveDataType.Name => MaskName(value),
                _ => value
            };
        }

        // 手机号脱敏 13812345678 -> 138****5678
        private string MaskPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone) || phone.Length < 11)
                return phone;

            return Regex.Replace(phone, @"(\d{3})\d{4}(\d{4})", "$1****$2");
        }

        // 手机号脱敏（支持自定义参数）
        private string MaskPhone(string phone, SensitiveDataAttribute attribute)
        {
            // 如果设置了自定义参数，则使用自定义规则
            if (attribute.PrefixKeep > 0 || attribute.SuffixKeep > 0)
            {
                return MaskWithParams(phone, attribute.PrefixKeep, attribute.SuffixKeep, attribute.MaskChar, attribute.MaskLength);
            }
            
            // 否则使用默认规则
            return MaskPhone(phone);
        }

        // 邮箱脱敏 example@domain.com -> ex***le@domain.com
        private string MaskEmail(string email)
        {
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

        // 邮箱脱敏（支持自定义参数）
        private string MaskEmail(string email, SensitiveDataAttribute attribute)
        {
            // 如果设置了自定义参数，则使用自定义规则
            if (attribute.PrefixKeep > 0 || attribute.SuffixKeep > 0)
            {
                var emailParts = email.Split('@');
                if (emailParts.Length != 2)
                    return email;

                var username = emailParts[0];
                var domain = emailParts[1];
                var maskedUsername = MaskWithParams(username, attribute.PrefixKeep, attribute.SuffixKeep, attribute.MaskChar, attribute.MaskLength);
                return maskedUsername + "@" + domain;
            }
            
            // 否则使用默认规则
            return MaskEmail(email);
        }

        // 身份证脱敏 110101199001011234 -> 110101********1234
        private string MaskIdCard(string idCard)
        {
            if (string.IsNullOrEmpty(idCard) || idCard.Length < 14)
                return idCard;

            return idCard.Substring(0, 6) + "********" + idCard.Substring(idCard.Length - 4);
        }

        // 身份证脱敏（支持自定义参数）
        private string MaskIdCard(string idCard, SensitiveDataAttribute attribute)
        {
            // 如果设置了自定义参数，则使用自定义规则
            if (attribute.PrefixKeep > 0 || attribute.SuffixKeep > 0)
            {
                return MaskWithParams(idCard, attribute.PrefixKeep, attribute.SuffixKeep, attribute.MaskChar, attribute.MaskLength);
            }
            
            // 否则使用默认规则
            return MaskIdCard(idCard);
        }

        // 银行卡脱敏 6222021234567890 -> 622202******7890
        private string MaskBankCard(string bankCard)
        {
            if (string.IsNullOrEmpty(bankCard) || bankCard.Length < 10)
                return bankCard;

            return bankCard.Substring(0, 6) + "******" + bankCard.Substring(bankCard.Length - 4);
        }

        // 银行卡脱敏（支持自定义参数）
        private string MaskBankCard(string bankCard, SensitiveDataAttribute attribute)
        {
            // 如果设置了自定义参数，则使用自定义规则
            if (attribute.PrefixKeep > 0 || attribute.SuffixKeep > 0)
            {
                return MaskWithParams(bankCard, attribute.PrefixKeep, attribute.SuffixKeep, attribute.MaskChar, attribute.MaskLength);
            }
            
            // 否则使用默认规则
            return MaskBankCard(bankCard);
        }

        // 姓名脱敏 张三 -> 张*
        private string MaskName(string name)
        {
            if (string.IsNullOrEmpty(name) || name.Length < 2)
                return name;

            if (name.Length == 2)
                return name.Substring(0, 1) + "*";

            var first = name.Substring(0, 1);
            var last = name.Substring(name.Length - 1, 1);
            var middle = new string('*', name.Length - 2);
            return first + middle + last;
        }

        // 姓名脱敏（支持自定义参数）
        private string MaskName(string name, SensitiveDataAttribute attribute)
        {
            // 如果设置了自定义参数，则使用自定义规则
            if (attribute.PrefixKeep > 0 || attribute.SuffixKeep > 0)
            {
                return MaskWithParams(name, attribute.PrefixKeep, attribute.SuffixKeep, attribute.MaskChar, attribute.MaskLength);
            }
            
            // 否则使用默认规则
            return MaskName(name);
        }

        // 自定义脱敏规则
        private string MaskCustom(string value, SensitiveDataAttribute attribute)
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
        private string MaskWithParams(string value, int prefixKeep, int suffixKeep, char maskChar, int maskLength)
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