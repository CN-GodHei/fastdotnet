using System;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Management; // 需要添加对 System.Management 的引用
using System.IO;
using System.Net.NetworkInformation;

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
