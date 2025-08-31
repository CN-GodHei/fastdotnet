using Fastdotnet.Plugin.Marketplace.Dto;

namespace Fastdotnet.Plugin.Marketplace.Services
{
    public interface ILicenseService
    {
        LicenseFileDto GenerateLicense(string pluginId, string userId, string licenseType, string machineFingerprint);
    }
}