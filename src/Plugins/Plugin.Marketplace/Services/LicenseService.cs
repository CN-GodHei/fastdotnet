using Fastdotnet.Plugin.Marketplace.Dto;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Cryptography;
using System.Text;
// using System.Text.Json; // 如果你之前用到了序列化，但服务本身不直接返回 JSON 字符串，可以移除

namespace Fastdotnet.Plugin.Marketplace.Services
{
    public class LicenseService : ILicenseService
    {
        private readonly RSA _privateKeyProvider;

        public LicenseService(IConfiguration configuration)
        {
            var privateKeyBase64 = configuration["Marketplace:PrivateKey"];
            if (string.IsNullOrEmpty(privateKeyBase64))
            {
                throw new InvalidOperationException("Marketplace:PrivateKey 未配置。请使用 user-secrets 进行设置。");
            }

            _privateKeyProvider = RSA.Create();
            _privateKeyProvider.ImportRSAPrivateKey(Convert.FromBase64String(privateKeyBase64), out _);
        }

        public LicenseFileDto GenerateLicense(string pluginId, string userId, string licenseType, string machineFingerprint)
        {
            var license = new LicenseFileDto
            {
                PluginId = pluginId ?? throw new ArgumentNullException(nameof(pluginId)),
                UserId = userId ?? throw new ArgumentNullException(nameof(userId)),
                LicenseType = licenseType ?? throw new ArgumentNullException(nameof(licenseType)),
                MachineFingerprint = machineFingerprint, // 可以为 null (对于 MultiServer)
                IssueDate = DateTime.UtcNow, // 使用 UTC 时间更标准
                UpdatesUntil = DateTime.UtcNow.AddYears(1) // 默认为1年的更新期限
            };

            // 构造要签名的数据字符串，必须与验证器的逻辑相匹配
            // 统一转为 Unix 时间戳（秒）以确保跨平台一致性
            long ToUtcTimestamp(DateTime dt) => ((DateTimeOffset)dt.ToUniversalTime()).ToUnixTimeSeconds();

            var dataToSign = string.Concat(
                license.PluginId,
                license.UserId,
                license.LicenseType,
                license.MachineFingerprint ?? "", // Handle null for MultiServer
                ToUtcTimestamp(license.IssueDate),
                ToUtcTimestamp(license.UpdatesUntil)
            );

            var dataBytes = Encoding.UTF8.GetBytes(dataToSign);

            // 对数据进行签名
            var signatureBytes = _privateKeyProvider.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            license.Signature = Convert.ToBase64String(signatureBytes);

            return license; // 返回 DTO 对象，由 ASP.NET Core 自动序列化为 JSON
        }
    }
}
