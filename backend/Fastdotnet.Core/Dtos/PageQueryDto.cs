namespace Fastdotnet.Core.Dtos
{
    /// <summary>
    /// 通用分页查询DTO
    /// </summary>
    public class PageQueryDto
    {
        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        /// <summary>
        /// 关键词，用于模糊搜索
        /// </summary>
        public string? Keyword { get; set; }
    }
}
