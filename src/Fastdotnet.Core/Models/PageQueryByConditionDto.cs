namespace Fastdotnet.Core.Models
{
    /// <summary>
    /// 通用分页查询DTO
    /// </summary>
    public class PageQueryByConditionDto
    {
        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        /// <summary>
        /// 关键词，用于模糊搜索
        /// </summary>
        public string? Keyword { get; set; }

        /// <summary>
        /// 动态查询条件，使用 System.Linq.Dynamic.Core 语法
        /// 例如: \"Age > 18 and Name.Contains(\\\"John\\\")
        /// </summary>
        public string? DynamicQuery { get; set; }
        /// <summary>
        /// 查询参数，与 DynamicQuery 配合使用
        /// 例如: new object[] { 18, \"John\" }
        /// </summary>
        public object[]? QueryParameters { get; set; }
    }
}
