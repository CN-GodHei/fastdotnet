
using Fastdotnet.Core.Entities.Sys;
using Fastdotnet.Core.Utils;
using Fastdotnet.Service.IService.Sys;

namespace Fastdotnet.Service.Service.Sys
{
    /// <summary>
    /// 密码服务实现
    /// </summary>
    public class PasswordService : IPasswordService
    {
        private readonly IRepository<FdDictData> _dictDataRepository;

        public PasswordService(IRepository<FdDictData> dictDataRepository)
        {
            _dictDataRepository = dictDataRepository;
        }

        /// <summary>
        /// 根据系统配置加密密码
        /// </summary>
        public async Task<string> EncryptPasswordAsync(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("密码不能为空", nameof(password));

            // 1. 获取密码加密类型
            var hashTypeConfig = await _dictDataRepository.GetFirstAsync(c => c.Code == "PasswordHashType");
            var hashTypeStr = hashTypeConfig?.Value ?? "Irreversible";
            var hashType = Enum.TryParse<CryptographyUtils.PasswordHashType>(hashTypeStr, out var parsedType)
                ? parsedType
                : CryptographyUtils.PasswordHashType.Irreversible;

            // 2. 如果是可逆加密，获取密钥
            string encryptionKey = null;
            if (hashType == CryptographyUtils.PasswordHashType.Reversible)
            {
                var keyConfig = await _dictDataRepository.GetFirstAsync(c => c.Code == "PasswordEncryptionKey");
                encryptionKey = keyConfig?.Value;

                if (string.IsNullOrEmpty(encryptionKey))
                {
                    throw new InvalidOperationException(
                        "密码加密密钥未配置，请在字典配置中添加 PASSWORD_CONFIG.PasswordEncryptionKey");
                }
            }

            // 3. 加密密码
            return CryptographyUtils.ProcessPassword(password, hashType, encryptionKey);
        }

        /// <summary>
        /// 获取默认加密密码
        /// 从字典读取默认密码并使用系统配置加密
        /// </summary>
        public async Task<string> GetDefaultEncryptedPasswordAsync()
        {
            // 1. 从字典读取默认密码
            var defaultPasswordConfig = await _dictDataRepository.GetFirstAsync(c => c.Code == "DefaultUserPassword");
            string defaultPassword = defaultPasswordConfig?.Value;

            if (string.IsNullOrEmpty(defaultPassword))
            {
                throw new InvalidOperationException(
                    "默认密码未配置，请在字典配置中添加 PASSWORD_CONFIG.DefaultUserPassword");
            }
            return await EncryptPasswordAsync(defaultPassword);
        }

        /// <summary>
        /// 验证密码
        /// </summary>
        public async Task<bool> VerifyPasswordAsync(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
                return false;

            // 1. 获取密码加密类型
            var hashTypeConfig = await _dictDataRepository.GetFirstAsync(c => c.Code == "PasswordHashType");
            var hashTypeStr = hashTypeConfig?.Value ?? "Irreversible";
            var hashType = Enum.TryParse<CryptographyUtils.PasswordHashType>(hashTypeStr, out var parsedType)
                ? parsedType
                : CryptographyUtils.PasswordHashType.Irreversible;

            // 2. 如果是可逆加密，获取密钥
            string encryptionKey = null;
            if (hashType == CryptographyUtils.PasswordHashType.Reversible)
            {
                var keyConfig = await _dictDataRepository.GetFirstAsync(c => c.Code == "PasswordEncryptionKey");
                encryptionKey = keyConfig?.Value;
            }

            // 3. 验证密码
            return CryptographyUtils.VerifyProcessedPassword(password, hashedPassword, hashType, encryptionKey);
        }
    }
}
