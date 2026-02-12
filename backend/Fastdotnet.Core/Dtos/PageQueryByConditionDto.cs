namespace Fastdotnet.Core.Dtos
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
        //public string? Keyword { get; set; }

        /// <summary>
        /// 动态查询条件，使用 System.Linq.Dynamic.Core 语法
        /// 例如: "Age > 18 and Name.Contains(\"John\")"
        /// </summary>
        public string? DynamicQuery { get; set; }
        
        /// <summary>
        /// 查询参数，与 DynamicQuery 配合使用
        /// 例如: new object[] { 18, "John" }
        /// </summary>
        public object[]? QueryParameters { get; set; }
        
    }

    public class QueryByConditionDto
    {

        /// <summary>
        /// 动态查询条件，使用 System.Linq.Dynamic.Core 语法
        /// 例如: "Age > 18 and Name.Contains(\"John\")"
        /// </summary>
        public string? DynamicQuery { get; set; }

        /// <summary>
        /// 查询参数，与 DynamicQuery 配合使用
        /// 例如: new object[] { 18, "John" }
        /// </summary>
        public object[]? QueryParameters { get; set; }

        /// <summary>
        /// 要返回的字段列表，例如: ["Id", "Name", "Email"]
        /// 如果为空或 null，则返回所有字段（即完整 TDto）
        /// </summary>
        public string[]? SelectFields { get; set; }
    }
}