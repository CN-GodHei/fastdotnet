using System;
using System.Security.Cryptography;
using System.Text;

namespace Fastdotnet.Plugin.Marketplace.Utils
{
    public static class FingerprintUtil
    {
        /// <summary>
        /// 为当前机器生成一个唯一的指纹。
        /// 注意：此实现必须与 Fastdotnet.Plugin.Core 中的实现保持完全一致。
        /// </summary>
        /// <returns>一个代表机器指纹的哈希字符串。</returns>
        public static string GenerateMachineFingerprint()
        {
            var machineName = Environment.MachineName;
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(machineName));
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
