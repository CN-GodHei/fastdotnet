using Fastdotnet.Plugin.Marketplace.Dto;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Fastdotnet.Plugin.Marketplace.Services
{

    public interface ILicenseService
    {
        LicenseFileDto GenerateLicense(string pluginId, string userId, string licenseType, string machineFingerprint);
    }

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
                PluginId = pluginId,
                UserId = userId,
                LicenseType = licenseType,
                MachineFingerprint = machineFingerprint,
                IssueDate = DateTime.Now,
                UpdatesUntil = DateTime.Now.AddYears(1) // 默认为1年的更新期限
            };
            // 构造要签名的数据字符串，必须与验证器的逻辑相匹配

            // 统一转为 Unix 时间戳（秒）
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

            // 对数据进行签名
            var signatureBytes = _privateKeyProvider.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            license.Signature = Convert.ToBase64String(signatureBytes);

            // 序列化为 JSON 字符串
            //return JsonSerializer.Serialize(license, new JsonSerializerOptions { WriteIndented = true });
            return license;
        }
    }
}
