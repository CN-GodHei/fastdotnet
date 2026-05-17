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
        /// 加密算法类型
        /// - "HYBRID": RSA + AES 混合加密（默认，推荐）
        /// - "RSA": 纯 RSA 加密（仅适用于极小数据）
        /// </summary>
        public string Algorithm { get; set; } = "HYBRID";

        /// <summary>
        /// 密钥标识
        /// </summary>
        public string KeyIdentifier { get; set; } = "";

        /// <summary>
        /// 初始化 <see cref="EncryptRequestAttribute"/> 类的新实例
        /// </summary>
        /// <param name="algorithm">加密算法（默认 HYBRID）</param>
        public EncryptRequestAttribute(string algorithm = "HYBRID")
        {
            Algorithm = algorithm;
        }
    }
}