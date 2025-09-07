namespace Fastdotnet.Core.Version
{
    /// <summary>
    /// 框架版本信息
    /// </summary>
    public static class FrameworkVersion
    {
        /// <summary>
        /// 主版本号
        /// </summary>
        public const int Major = 1;

        /// <summary>
        /// 次版本号
        /// </summary>
        public const int Minor = 0;

        /// <summary>
        /// 修订号
        /// </summary>
        public const int Patch = 0;

        /// <summary>
        /// 获取完整的版本号字符串
        /// </summary>
        public static string FullVersion => $"{Major}.{Minor}.{Patch}";

        /// <summary>
        /// 检查版本兼容性
        /// </summary>
        /// <param name="requiredVersion">所需版本号</param>
        /// <returns>是否兼容</returns>
        public static bool IsCompatibleWith(string requiredVersion)
        {
            if (string.IsNullOrEmpty(requiredVersion))
                return false;

            var parts = requiredVersion.Split('.');
            if (parts.Length != 3)
                return false;

            if (!int.TryParse(parts[0], out int requiredMajor) ||
                !int.TryParse(parts[1], out int requiredMinor) ||
                !int.TryParse(parts[2], out int requiredPatch))
                return false;

            // 主版本号必须完全匹配
            if (requiredMajor != Major)
                return false;

            // 次版本号和修订号可以高于要求
            if (Minor < requiredMinor)
                return false;

            if (Minor == requiredMinor && Patch < requiredPatch)
                return false;

            return true;
        }
    }
}