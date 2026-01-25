using System;
using System.Reflection;

namespace Fastdotnet.Core.Attributes
{
    /// <summary>
    /// 加密特性帮助类
    /// </summary>
    public static class EncryptionAttributeHelper
    {
        /// <summary>
        /// 检查方法或类型是否启用了请求加密
        /// </summary>
        /// <param name="methodOrType">方法信息或类型信息</param>
        /// <returns>是否启用了请求加密</returns>
        public static bool IsRequestEncryptionEnabled(MemberInfo methodOrType)
        {
            var attr = methodOrType.GetCustomAttribute<EncryptRequestAttribute>();
            if (attr != null)
            {
                return true;
            }

            // 如果方法上没有找到特性，尝试从类型上查找
            if (methodOrType is MethodInfo methodInfo)
            {
                attr = methodInfo.DeclaringType?.GetCustomAttribute<EncryptRequestAttribute>();
            }
            else if (methodOrType is Type type)
            {
                attr = type.GetCustomAttribute<EncryptRequestAttribute>();
            }

            return attr != null;
        }

        /// <summary>
        /// 检查方法或类型是否启用了响应加密
        /// </summary>
        /// <param name="methodOrType">方法信息或类型信息</param>
        /// <returns>是否启用了响应加密</returns>
        public static bool IsResponseEncryptionEnabled(MemberInfo methodOrType)
        {
            var attr = methodOrType.GetCustomAttribute<EncryptResponseAttribute>();
            if (attr != null)
            {
                return true;
            }

            // 如果方法上没有找到特性，尝试从类型上查找
            if (methodOrType is MethodInfo methodInfo)
            {
                attr = methodInfo.DeclaringType?.GetCustomAttribute<EncryptResponseAttribute>();
            }
            else if (methodOrType is Type type)
            {
                attr = type.GetCustomAttribute<EncryptResponseAttribute>();
            }

            return attr != null;
        }

        /// <summary>
        /// 获取方法或类型的请求加密算法
        /// </summary>
        /// <param name="methodOrType">方法信息或类型信息</param>
        /// <returns>加密算法名称</returns>
        public static string GetRequestEncryptionAlgorithm(MemberInfo methodOrType)
        {
            var attr = methodOrType.GetCustomAttribute<EncryptRequestAttribute>();
            if (attr != null)
            {
                return attr.Algorithm;
            }

            // 如果方法上没有找到特性，尝试从类型上查找
            if (methodOrType is MethodInfo methodInfo)
            {
                attr = methodInfo.DeclaringType?.GetCustomAttribute<EncryptRequestAttribute>();
            }
            else if (methodOrType is Type type)
            {
                attr = type.GetCustomAttribute<EncryptRequestAttribute>();
            }

            return attr?.Algorithm ?? "SM2"; // 默认使用SM2
        }

        /// <summary>
        /// 获取方法或类型的响应加密算法
        /// </summary>
        /// <param name="methodOrType">方法信息或类型信息</param>
        /// <returns>加密算法名称</returns>
        public static string GetResponseEncryptionAlgorithm(MemberInfo methodOrType)
        {
            var attr = methodOrType.GetCustomAttribute<EncryptResponseAttribute>();
            if (attr != null)
            {
                return attr.Algorithm;
            }

            // 如果方法上没有找到特性，尝试从类型上查找
            if (methodOrType is MethodInfo methodInfo)
            {
                attr = methodInfo.DeclaringType?.GetCustomAttribute<EncryptResponseAttribute>();
            }
            else if (methodOrType is Type type)
            {
                attr = type.GetCustomAttribute<EncryptResponseAttribute>();
            }

            return attr?.Algorithm ?? "SM2"; // 默认使用SM2
        }

        /// <summary>
        /// 获取方法或类型的请求加密密钥标识
        /// </summary>
        /// <param name="methodOrType">方法信息或类型信息</param>
        /// <returns>密钥标识</returns>
        public static string GetRequestEncryptionKeyIdentifier(MemberInfo methodOrType)
        {
            var attr = methodOrType.GetCustomAttribute<EncryptRequestAttribute>();
            if (attr != null)
            {
                return attr.KeyIdentifier;
            }

            // 如果方法上没有找到特性，尝试从类型上查找
            if (methodOrType is MethodInfo methodInfo)
            {
                attr = methodInfo.DeclaringType?.GetCustomAttribute<EncryptRequestAttribute>();
            }
            else if (methodOrType is Type type)
            {
                attr = type.GetCustomAttribute<EncryptRequestAttribute>();
            }

            return attr?.KeyIdentifier ?? "";
        }

        /// <summary>
        /// 获取方法或类型的响应加密密钥标识
        /// </summary>
        /// <param name="methodOrType">方法信息或类型信息</param>
        /// <returns>密钥标识</returns>
        public static string GetResponseEncryptionKeyIdentifier(MemberInfo methodOrType)
        {
            var attr = methodOrType.GetCustomAttribute<EncryptResponseAttribute>();
            if (attr != null)
            {
                return attr.KeyIdentifier;
            }

            // 如果方法上没有找到特性，尝试从类型上查找
            if (methodOrType is MethodInfo methodInfo)
            {
                attr = methodInfo.DeclaringType?.GetCustomAttribute<EncryptResponseAttribute>();
            }
            else if (methodOrType is Type type)
            {
                attr = type.GetCustomAttribute<EncryptResponseAttribute>();
            }

            return attr?.KeyIdentifier ?? "";
        }
    }
}