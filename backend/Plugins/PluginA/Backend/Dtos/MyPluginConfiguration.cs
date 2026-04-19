namespace Plugina.Dtos
{
    /// <summary>
    /// 插件配置类示例
    /// 请根据实际需求修改此类
    /// </summary>
    public class MyPluginConfiguration
    {
        /// <summary>
        /// 配置项示例 1
        /// </summary>
        public string Setting1 { get; set; } = "DefaultValue";

        /// <summary>
        /// 配置项示例 2
        /// </summary>
        public int Setting2 { get; set; } = 100;

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// 嵌套配置示例
        /// </summary>
        public SubConfiguration? SubSettings { get; set; }
    }

    /// <summary>
    /// 嵌套配置示例
    /// </summary>
    public class SubConfiguration
    {
        public string OptionA { get; set; } = string.Empty;
        public int OptionB { get; set; } = 0;
        public bool OptionC { get; set; } = false;
    }
}
