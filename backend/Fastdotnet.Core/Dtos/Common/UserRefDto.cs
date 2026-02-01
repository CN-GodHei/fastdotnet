

namespace Fastdotnet.Core.Dtos.Common
{
    /// <summary>
    /// 用户引用信息（用于DTO中代替原始UserId）
    /// </summary>
    public class UserRefDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 用户姓名（或显示名）
        /// </summary>
        public string Name { get; set; } = "未知用户";
    }
}
