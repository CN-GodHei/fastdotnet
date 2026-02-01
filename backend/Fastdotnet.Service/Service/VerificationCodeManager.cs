
namespace Fastdotnet.Service.Service
{
    public class VerificationCodeManager : IVerificationCodeManager
    {
        private readonly IReadOnlyDictionary<string, IVerificationCodeStrategy> _strategies;

        public VerificationCodeManager(IEnumerable<IVerificationCodeStrategy> strategies)
        {
            // 将所有策略转为字典，以业务码为Key，方便快速查找
            _strategies = strategies.ToDictionary(s => s.BusinessCode, s => s);
        }

        public async Task SendCodeAsync(string recipient, string businessCode, int? expirationInMinutes = null)
        {
            var strategy = GetStrategy(businessCode);
            await strategy.SendAsync(recipient, expirationInMinutes);
        }

        public async Task<bool> VerifyCodeAsync(string recipient, string code, string businessCode)
        {
            var strategy = GetStrategy(businessCode);
            return await strategy.VerifyAsync(recipient, code);
        }

        private IVerificationCodeStrategy GetStrategy(string businessCode)
        {
            // 如果传入的businessCode为空，则使用"Default"作为业务码
            string lookupCode = string.IsNullOrWhiteSpace(businessCode) ? "Default" : businessCode;

            if (_strategies.TryGetValue(lookupCode, out var strategy))
            {
                return strategy;
            }

            throw new BusinessException($"未找到能处理业务码 '{lookupCode}' 的验证码策略。");
        }
    }
}
