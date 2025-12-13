namespace Fastdotnet.WebApi.Filters
{
    /// <summary>
    /// 控制Swagger标签排序的文档过滤器
    /// </summary>
    public class TagOrderDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            // 定义期望的标签顺序
            var desiredTagOrder = new List<string>
            {
                //"00-认证接口",
                //"认证接口",
                "Auth"
            };

            // 获取所有现有的标签
            var tags = swaggerDoc.Tags?.ToList() ?? new List<OpenApiTag>();

            // 根据期望的顺序重新排列标签
            var orderedTags = tags.OrderBy(tag =>
            {
                var index = desiredTagOrder.IndexOf(tag.Name);
                return index == -1 ? int.MaxValue : index; // 未指定顺序的标签放到最后
            }).ToList();

            // 更新文档的标签顺序
            swaggerDoc.Tags = orderedTags;
        }
    }
}