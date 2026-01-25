using System;

namespace Fastdotnet.Core.Attributes
{
    /// <summary>
    /// 标记接口请求参数需要加密的特性
    /// 可应用于类或方法级别
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class EncryptRequestAttribute : Attribute
    {
        /// <summary>
        /// 是否启用请求加密
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// 加密算法类型
        /// </summary>
        public string Algorithm { get; set; } = "SM2";

        /// <summary>
        /// 密钥标识
        /// </summary>
        public string KeyIdentifier { get; set; } = "";

        /// <summary>
        /// 初始化 <see cref="EncryptRequestAttribute"/> 类的新实例
        /// </summary>
        public EncryptRequestAttribute()
        {
        }

        /// <summary>
        /// 初始化 <see cref="EncryptRequestAttribute"/> 类的新实例
        /// </summary>
        /// <param name="enabled">是否启用加密</param>
        public EncryptRequestAttribute(bool enabled)
        {
            Enabled = enabled;
        }

        /// <summary>
        /// 初始化 <see cref="EncryptRequestAttribute"/> 类的新实例
        /// </summary>
        /// <param name="algorithm">加密算法</param>
        public EncryptRequestAttribute(string algorithm)
        {
            Algorithm = algorithm;
        }

        /// <summary>
        /// 初始化 <see cref="EncryptRequestAttribute"/> 类的新实例
        /// </summary>
        /// <param name="enabled">是否启用加密</param>
        /// <param name="algorithm">加密算法</param>
        public EncryptRequestAttribute(bool enabled, string algorithm)
        {
            Enabled = enabled;
            Algorithm = algorithm;
        }
    }
}