using System;
using System.IO;
using System.Management;
using System.Net.NetworkInformation;
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
        //public static string GenerateMachineFingerprint()
        //{
        //    // 基础示例：使用机器名。
        //    // 更好的实现是使用 System.Management 获取硬件ID。
        //    var machineName = Environment.MachineName;
        //    using (var sha256 = SHA256.Create())
        //    {
        //        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(machineName));
        //        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        //    }
        //}

        public static string GenerateMachineFingerprint()
        {
            var sb = new StringBuilder();

            // 收集硬件信息
            bool hasHardwareInfo = false;
            try
            {
                // 1. CPU ID
                using (var searcher = new ManagementObjectSearcher("SELECT ProcessorId FROM Win32_Processor"))
                {
                    foreach (ManagementObject mo in searcher.Get())
                    {
                        var id = mo["ProcessorId"]?.ToString();
                        if (!string.IsNullOrEmpty(id))
                        {
                            sb.Append(id);
                            hasHardwareInfo = true;
                        }
                    }
                }

                // 2. 主板序列号
                using (var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BaseBoard"))
                {
                    foreach (ManagementObject mo in searcher.Get())
                    {
                        var sn = mo["SerialNumber"]?.ToString();
                        if (!string.IsNullOrEmpty(sn))
                        {
                            sb.Append(sn);
                            hasHardwareInfo = true;
                        }
                    }
                }

                // 3. BIOS ID (可选，但推荐)
                using (var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BIOS"))
                {
                    foreach (ManagementObject mo in searcher.Get())
                    {
                        var sn = mo["SerialNumber"]?.ToString();
                        if (!string.IsNullOrEmpty(sn))
                        {
                            sb.Append(sn);
                            hasHardwareInfo = true;
                        }
                    }
                }

                // 4. 磁盘驱动器序列号 (可选，但推荐)
                // 使用第一个固定硬盘的序列号
                using (var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_DiskDrive WHERE MediaType LIKE '%Fixed%'"))
                {
                    foreach (ManagementObject mo in searcher.Get().Cast<ManagementObject>().Take(1))
                    {
                        var sn = mo["SerialNumber"]?.ToString();
                        if (!string.IsNullOrEmpty(sn))
                        {
                            sb.Append(sn);
                            hasHardwareInfo = true;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                // 记录警告日志（如果系统有日志功能）
                // Console.WriteLine($"Warning: Could not retrieve some hardware identifiers for fingerprint: {ex.Message}");
                // 不立即回退，继续收集其他信息
            }

            // 即使没有硬件信息，也收集其他系统信息来增强指纹
            try
            {
                // 5. Machine Name
                sb.Append(Environment.MachineName ?? "");

                // 6. User Domain Name (如果有的话)
                sb.Append(Environment.UserDomainName ?? "");

                // 7. OS Version
                sb.Append(Environment.OSVersion?.ToString() ?? "");

                // 8. System Directory
                sb.Append(Environment.SystemDirectory ?? "");

                // 9. 系统驱动器的卷标或ID (通常不需要特殊权限)
                var systemDrive = Path.GetPathRoot(Environment.SystemDirectory);
                if (systemDrive != null)
                {
                    var driveInfo = new DriveInfo(systemDrive);
                    sb.Append(driveInfo?.VolumeLabel ?? driveInfo?.Name ?? ""); // 卷标或驱动器名
                }

                // 10. 首个网络适配器的 MAC 地址 (通常不需要特殊权限)
                // 注意：MAC 地址在更换网卡或使用 WiFi 时可能会变化
                var firstMacAddress = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    .Select(nic => nic.GetPhysicalAddress().ToString())
                    .FirstOrDefault();
                sb.Append(firstMacAddress ?? "");

            }
            catch (Exception ex)
            {
                // 记录警告日志
                // Console.WriteLine($"Warning: Could not retrieve additional system identifiers for fingerprint: {ex.Message}");
            }

            // 如果完全没有收集到任何信息，则使用一个默认值（理论上不太可能发生）
            if (sb.Length == 0)
            {
                sb.Append("DEFAULT_FINGERPRINT_FALLBACK");
            }

            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
