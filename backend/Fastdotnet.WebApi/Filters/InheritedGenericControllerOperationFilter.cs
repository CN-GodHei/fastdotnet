namespace Fastdotnet.WebApi.Filters
{
    /// <summary>
    /// 为继承自 GenericDtoControllerBase 的控制器方法自动生成 Swagger 文档摘要和描述
    /// </summary>
    public class InheritedGenericControllerOperationFilter : IOperationFilter
    {
        private readonly XDocument _xmlDoc;
        private readonly ConcurrentDictionary<string, string> _memberSummaryCache = new();

        public InheritedGenericControllerOperationFilter()
        {
            // 尝试加载 WebApi 项目的 XML 注释文件
            try
            {
                var xmlPath = Path.Combine(AppContext.BaseDirectory, "Fastdotnet.WebApi.xml");
                if (File.Exists(xmlPath))
                {
                    _xmlDoc = XDocument.Load(xmlPath);
                }
            }
            catch (Exception ex)
            {
                // 记录日志或处理异常，但不要让过滤器失败
                Console.WriteLine($"Failed to load XML comments for InheritedGenericControllerOperationFilter: {ex.Message}");
                _xmlDoc = null;
            }
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var methodInfo = context.MethodInfo;
            var controllerType = methodInfo.DeclaringType;

            // 如果操作已经有摘要（可能来自子类的直接注释），则不覆盖
            if (!string.IsNullOrWhiteSpace(operation.Summary))
            {
                return;
            }

            // 检查控制器是否继承自 GenericDtoControllerBase<,,,,> (5个泛型参数)
            if (controllerType != null && IsSubclassOfGeneric(controllerType, typeof(GenericDtoControllerBase<,,,,>)))
            {
                ApplyDocumentation(operation, methodInfo, controllerType, 5);
            }
            // 检查控制器是否继承自 GenericDtoControllerBase<,,,> (4个泛型参数, 默认主键为string)
            else if (controllerType != null && IsSubclassOfGeneric(controllerType, typeof(GenericDtoControllerBase<,,,>)))
            {
                ApplyDocumentation(operation, methodInfo, controllerType, 4);
            }
        }

        private void ApplyDocumentation(OpenApiOperation operation, MethodInfo methodInfo, Type controllerType, int genericTypeParamCount)
        {
            // 检查子类方法是否有自定义的 <summary>
            string customSummary = GetCustomSummary(methodInfo);
            if (!string.IsNullOrWhiteSpace(customSummary))
            {
                // 如果子类有自定义摘要，则使用它
                operation.Summary = customSummary;
                // 这里可以进一步解析自定义注释中的 <remarks>, <response> 等，但通常Swagger会自动处理
                // 我们这里只简单替换 Summary 作为示例
                return;
            }

            // 如果没有自定义摘要，则应用通用的
            switch (methodInfo.Name)
            {
                case "GetAll":
                    operation.Summary = "获取所有记录";
                    operation.Description = "检索并返回系统中该类型的所有记录。";
                    break;

                case "GetById":
                    operation.Summary = "根据ID获取记录";
                    operation.Description = "根据提供的唯一标识符(ID)检索特定记录的详细信息。";
                    if (operation.Parameters.FirstOrDefault(p => p.Name == "id") is OpenApiParameter idParam)
                    {
                        idParam.Description = "记录的唯一标识符";
                    }
                    break;

                case "GetPage":
                    operation.Summary = "分页获取记录";
                    operation.Description = "根据页码和页面大小，分页检索记录。";
                    var pageIndexParam = operation.Parameters.FirstOrDefault(p => p.Name == "pageIndex");
                    if (pageIndexParam != null)
                    {
                        pageIndexParam.Description = "页码 (从1开始)";
                    }
                    var pageSizeParam = operation.Parameters.FirstOrDefault(p => p.Name == "pageSize");
                    if (pageSizeParam != null)
                    {
                        pageSizeParam.Description = "页面大小";
                    }
                    break;

                case "GetPageByCondition":
                    operation.Summary = "根据条件分页获取记录";
                    operation.Description = "根据提供的查询条件和分页参数，分页检索记录。";
                    var searchPageIndexParam = operation.Parameters.FirstOrDefault(p => p.Name == "pageIndex");
                    if (searchPageIndexParam != null)
                    {
                        searchPageIndexParam.Description = "页码 (从1开始)";
                    }
                    var searchPageSizeParam = operation.Parameters.FirstOrDefault(p => p.Name == "pageSize");
                    if (searchPageSizeParam != null)
                    {
                        searchPageSizeParam.Description = "页面大小";
                    }
                    break;

                case "Create":
                    operation.Summary = "创建新记录";
                    operation.Description = "根据提供的数据创建一条新记录。";
                    var createBodyParam = operation.RequestBody;
                    if (createBodyParam != null)
                    {
                        createBodyParam.Description = "创建记录所需的数据";
                    }
                    break;
                case "CreateMany":
                    operation.Summary = "批量创建新记录";
                    operation.Description = "根据提供的数据批量创建新记录。";
                    var CreateManyBodyParam = operation.RequestBody;
                    if (CreateManyBodyParam != null)
                    {
                        CreateManyBodyParam.Description = "创建记录所需的数据";
                    }
                    break;
                case "Update":
                    operation.Summary = "更新现有记录";
                    operation.Description = "根据提供的ID和更新数据，修改现有记录的信息。";
                    if (operation.Parameters.FirstOrDefault(p => p.Name == "id") is OpenApiParameter updateIdParam)
                    {
                        updateIdParam.Description = "要更新的记录的唯一标识符";
                    }
                    var updateBodyParam = operation.RequestBody;
                    if (updateBodyParam != null)
                    {
                        updateBodyParam.Description = "更新记录所需的数据";
                    }
                    break;
                case "UpdateMany":
                    operation.Summary = "根据实体主键批量更新实体信息";
                    operation.Description = "根据实体主键批量更新实体信息";
                    var UpdateManyBodyParam = operation.RequestBody;
                    if (UpdateManyBodyParam != null)
                    {
                        UpdateManyBodyParam.Description = "批量更新实体数据";
                    }
                    break;
                case "UpdateManyByCondition":
                    operation.Summary = "根据条件批量更新实体属性（部分字段更新）";
                    operation.Description = "根据条件批量更新实体属性（部分字段更新）";
                    var UpdateManyByConditionBodyParam = operation.RequestBody;
                    if (UpdateManyByConditionBodyParam != null)
                    {
                        UpdateManyByConditionBodyParam.Description = "根据条件批量更新实体属性（部分字段更新）";
                    }
                    break;

                case "Delete":
                    operation.Summary = "删除记录";
                    operation.Description = "根据提供的ID，从系统中移除指定的记录。";
                    if (operation.Parameters.FirstOrDefault(p => p.Name == "id") is OpenApiParameter deleteIdParam)
                    {
                        deleteIdParam.Description = "要删除的记录的唯一标识符";
                    }
                    break;

                case "BatchDelete":
                    operation.Summary = "批量删除记录";
                    operation.Description = "根据提供的ID列表，批量删除多条记录。";
                    var batchDeleteBodyParam = operation.RequestBody;
                    if (batchDeleteBodyParam != null)
                    {
                        batchDeleteBodyParam.Description = "要删除的记录ID列表";
                    }
                    break;

                // 回收站相关方法
                case "GetRecycleBin":
                    operation.Summary = "获取回收站数据";
                    operation.Description = "检索并返回已软删除的记录（回收站数据）。";
                    var recycleBinPageIndexParam = operation.Parameters.FirstOrDefault(p => p.Name == "pageIndex");
                    if (recycleBinPageIndexParam != null)
                    {
                        recycleBinPageIndexParam.Description = "页码 (从1开始)";
                    }
                    var recycleBinPageSizeParam = operation.Parameters.FirstOrDefault(p => p.Name == "pageSize");
                    if (recycleBinPageSizeParam != null)
                    {
                        recycleBinPageSizeParam.Description = "页面大小";
                    }
                    break;

                case "SearchRecycleBin":
                    operation.Summary = "根据条件查询回收站数据";
                    operation.Description = "根据提供的查询条件，检索回收站中的记录。";
                    var searchRecycleBinPageIndexParam = operation.Parameters.FirstOrDefault(p => p.Name == "pageIndex");
                    if (searchRecycleBinPageIndexParam != null)
                    {
                        searchRecycleBinPageIndexParam.Description = "页码 (从1开始)";
                    }
                    var searchRecycleBinPageSizeParam = operation.Parameters.FirstOrDefault(p => p.Name == "pageSize");
                    if (searchRecycleBinPageSizeParam != null)
                    {
                        searchRecycleBinPageSizeParam.Description = "页面大小";
                    }
                    break;

                case "Restore":
                    operation.Summary = "恢复回收站中的记录";
                    operation.Description = "根据提供的ID，将已软删除的记录恢复到正常状态。";
                    if (operation.Parameters.FirstOrDefault(p => p.Name == "id") is OpenApiParameter restoreIdParam)
                    {
                        restoreIdParam.Description = "要恢复的记录的唯一标识符";
                    }
                    break;

                case "RestoreBatch":
                    operation.Summary = "批量恢复回收站中的记录";
                    operation.Description = "根据提供的条件，批量将回收站中的记录恢复到正常状态。";
                    var restoreBatchBodyParam = operation.RequestBody;
                    if (restoreBatchBodyParam != null)
                    {
                        restoreBatchBodyParam.Description = "用于筛选要恢复记录的条件表达式";
                    }
                    break;

                case "PermanentDelete":
                    operation.Summary = "永久删除回收站中的记录";
                    operation.Description = "根据提供的ID，将已软删除的记录从数据库中永久移除。";
                    if (operation.Parameters.FirstOrDefault(p => p.Name == "id") is OpenApiParameter permDeleteIdParam)
                    {
                        permDeleteIdParam.Description = "要永久删除的记录的唯一标识符";
                    }
                    break;

                case "PermanentDeleteBatch":
                    operation.Summary = "根据条件永久删除回收站中的记录";
                    operation.Description = "根据提供的条件，将回收站中符合条件的记录从数据库中永久移除。";
                    var permDeleteBatchBodyParam = operation.RequestBody;
                    if (permDeleteBatchBodyParam != null)
                    {
                        permDeleteBatchBodyParam.Description = "用于筛选要永久删除记录的条件表达式";
                    }
                    break;
            }
        }

        /// <summary>
        /// 从XML注释中获取方法的自定义摘要
        /// </summary>
        private string GetCustomSummary(MethodInfo methodInfo)
        {
            if (_xmlDoc == null) return null;

            // 构建XML中成员的名称，例如：M:Namespace.Controller.MethodName(System.String,System.Int32)
            string memberName = XmlCommentsMemberNameHelper.GetMemberNameForMethod(methodInfo);
            
            // 使用缓存提高性能
            if (_memberSummaryCache.TryGetValue(memberName, out string cachedSummary))
            {
                return cachedSummary;
            }

            // 查找对应的 member 节点
            var memberElement = _xmlDoc.Root?.Element("members")?.Elements("member")
                .FirstOrDefault(m => m.Attribute("name")?.Value == memberName);

            if (memberElement == null) return null;

            // 提取 <summary> 节点的文本
            var summaryElement = memberElement.Element("summary");
            string summary = summaryElement?.Value.Trim();

            // 缓存结果
            _memberSummaryCache[memberName] = summary;
            return summary;
        }

        /// <summary>
        /// 检查类型是否继承自指定的泛型基类
        /// </summary>
        private static bool IsSubclassOfGeneric(Type toCheck, Type generic)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }
    }

    // 内部辅助类，用于生成XML注释中的成员名称
    internal static class XmlCommentsMemberNameHelper
    {
        public static string GetMemberNameForMethod(MethodInfo method)
        {
            var builder = new System.Text.StringBuilder("M:");

            var declaringType = method.DeclaringType;
            AppendFullTypeName(builder, declaringType);

            builder.Append('.');
            builder.Append(method.Name);

            var parameters = method.GetParameters();
            if (parameters.Length > 0)
            {
                builder.Append('(');
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (i > 0) builder.Append(',');
                    AppendFullTypeName(builder, parameters[i].ParameterType);
                }
                builder.Append(')');
            }

            return builder.ToString();
        }

        private static void AppendFullTypeName(System.Text.StringBuilder builder, Type type)
        {
            // 处理泛型类型
            if (type.IsGenericType)
            {
                var fullName = type.GetGenericTypeDefinition().FullName;
                // 将 `N 替换为 'N (例如, System.Collections.Generic.List`1 -> System.Collections.Generic.List'1)
                builder.Append(fullName.Replace('+', '.').Replace('`', '\''));
                builder.Append('{');
                var args = type.GetGenericArguments();
                for (int i = 0; i < args.Length; i++)
                {
                    if (i > 0) builder.Append(',');
                    AppendFullTypeName(builder, args[i]);
                }
                builder.Append('}');
            }
            else
            {
                builder.Append(type.FullName?.Replace('+', '.') ?? type.Name);
            }
        }
    }
}