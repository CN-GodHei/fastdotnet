using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Fastdotnet.Plugin.Core.Infrastructure;
using Newtonsoft.Json;

namespace Fastdotnet.Plugin.Core.Security
{
    /// <summary>
    /// 表示插件授权文件的结构。
    /// </summary>
    public class LicenseFile
    {
        public string PluginId { get; set; }
        public string UserId { get; set; }
        public string LicenseType { get; set; }
        public string MachineFingerprint { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime UpdatesUntil { get; set; }
        public string Signature { get; set; }
    }

    /// <summary>
    /// 提供用于验证插件授权的服务。
    /// </summary>
    public class LicenseValidator
    {
        private readonly RSA _publicKeyProvider;

        public LicenseValidator()
        {
            _publicKeyProvider = RSA.Create();
            _publicKeyProvider.ImportRSAPublicKey(Convert.FromBase64String(LicensePublicKey.Value), out _);
        }

        /// <summary>
        /// 验证给定插件的授权。
        /// </summary>
        /// <param name="pluginDirectory">插件的根目录。</param>
        /// <returns>如果授权有效，则返回 true；否则返回 false。</returns>
        public bool Validate(string pluginDirectory)
        {
            var licenseFilePath = Path.Combine(pluginDirectory, "plugin.lic");

            if (!File.Exists(licenseFilePath))
            {
                // TODO: 记录授权文件缺失的日志。
                return false;
            }

            try
            {
                var licenseJson = File.ReadAllText(licenseFilePath);
                //var license = JsonSerializer.Deserialize<LicenseFile>(licenseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                var license = JsonConvert.DeserializeObject<LicenseFile>(licenseJson, new JsonSerializerSettings
                {
                    // Newtonsoft.Json 默认就是不区分大小写的！
                    // 所以通常不需要额外设置即可匹配 PascalCase/CamelCase
                });
                if (license == null || string.IsNullOrEmpty(license.Signature))
                {
                    // TODO: 记录授权格式无效的日志。
                    return false;
                }

                // 1. 验证签名
                if (!IsSignatureValid(license))
                {
                    // TODO: 记录签名验证失败的日志。
                    return false;
                }

                // 2. 验证机器指纹（如果适用）
                if (license.LicenseType?.Equals("SingleServer", StringComparison.OrdinalIgnoreCase) == true)
                {
                    if (!IsMachineFingerprintValid(license.MachineFingerprint))
                    {
                        // TODO: 记录机器指纹不匹配的日志。
                        return false;
                    }
                }

                // TODO: 添加其他检查，如过期时间、插件ID匹配等。

                return true;
            }
            catch (Exception)
            {
                // TODO: 记录授权验证期间的异常日志。
                return false;
            }
        }

        private bool IsSignatureValid(LicenseFile license)
        {
            var signature = Convert.FromBase64String(license.Signature);
            // 以与服务器完全相同的方式构造要签名的数据字符串。
            long ToUtcTimestamp(DateTime dt) =>
    ((DateTimeOffset)dt.ToUniversalTime()).ToUnixTimeSeconds();

            var dataToSign = string.Concat(
                license.PluginId,
                license.UserId,
                license.LicenseType,
                license.MachineFingerprint,
                ToUtcTimestamp(license.IssueDate),
                ToUtcTimestamp(license.UpdatesUntil)
            );
            //var dataToSign = $"{license.PluginId}{license.UserId}{license.LicenseType}{license.MachineFingerprint}{license.IssueDate:O}{license.UpdatesUntil:O}";
            var dataBytes = Encoding.UTF8.GetBytes(dataToSign);

            return _publicKeyProvider.VerifyData(dataBytes, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }

        private bool IsMachineFingerprintValid(string licenseFingerprint)
        {
            if (string.IsNullOrEmpty(licenseFingerprint))
            {
                return false;
            }
            
            var currentFingerprint = GenerateMachineFingerprint();
            return currentFingerprint.Equals(licenseFingerprint, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 为当前机器生成一个唯一的指纹。
        /// 注意：这是一个基础实现。在生产环境中，应考虑使用更健壮的方法，
        /// 结合多个硬件标识符（如 CPU ID、主板序列号等）。
        /// </summary>
        /// <returns>一个代表机器指纹的哈希字符串。</returns>
        public static string GenerateMachineFingerprint()
        {
            // 基础示例：使用机器名。
            // 更好的实现是使用 System.Management 获取硬件ID。
            var machineName = Environment.MachineName;
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(machineName));
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
