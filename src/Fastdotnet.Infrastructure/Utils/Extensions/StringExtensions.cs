using System;
using System.Security.Cryptography;
using System.Text;

namespace Fastdotnet.Infrastructure.Utils.Extensions
{
    /// <summary>
    /// 字符串扩展方法
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="str">待加密字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string ToMD5(this string str)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.UTF8.GetBytes(str);
                var hashBytes = md5.ComputeHash(inputBytes);
                var sb = new StringBuilder();
                foreach (var b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// 判断字符串是否为空或空白字符
        /// </summary>
        /// <param name="str">待判断字符串</param>
        /// <returns>是否为空</returns>
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="length">截取长度</param>
        /// <param name="suffix">后缀</param>
        /// <returns>截取后的字符串</returns>
        public static string Cut(this string str, int length, string suffix = "...")
        {
            if (str.IsNullOrWhiteSpace() || str.Length <= length)
            {
                return str;
            }
            return str.Substring(0, length) + suffix;
        }
    }
}