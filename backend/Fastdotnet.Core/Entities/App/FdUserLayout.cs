using SqlSugar;

namespace Fastdotnet.Core.Entities.App
{
    /// <summary>
    /// 用户个人工作台布局表 (应用端)
    /// </summary>
    [SugarTable("fd_user_layout")]
    public class FdUserLayout : BaseEntity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; } = null!;

        /// <summary>
        /// 布局 JSON 数据 (包含卡片 ID、位置 x, y, w, h)
        /// </summary>
        [SugarColumn(ColumnDataType = "text")]
        public string LayoutData { get; set; } = null!;

        /// <summary>
        /// 布局名称 (支持多套布局切换)
        /// </summary>
        public string Name { get; set; } = "Default";

        /// <summary>
        /// 是否为当前激活布局
        /// </summary>
        public int IsActive { get; set; } = 1;
    }
}
