
namespace Fastdotnet.Core.Enum
{
    /// <summary>
    /// 数据状态
    /// </summary>
    [Description("数据状态")]
    public enum DataStatus
    {
        /// <summary>
        /// 无变化
        /// </summary>
        [Description("无变化")]
        NoChange = 0,

        /// <summary>
        /// 新增
        /// </summary>
        [Description("新增")]
        Added = 1,

        /// <summary>
        /// 修改
        /// </summary>
        [Description("修改")]
        Modified = 2,

        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除")]
        Deleted = 3,
    }
}
