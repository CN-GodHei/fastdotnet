using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;
using System.Reflection;
using Fastdotnet.Core.Controllers; // 确保引用了包含 GenericDtoControllerBase 的命名空间
using Fastdotnet.Core.Models.Base; // 引用 BaseEntity

namespace Fastdotnet.WebApi.Filters
{
    /// <summary>
    /// 为继承自 GenericDtoControllerBase 的控制器方法自动生成 Swagger 文档摘要和描述
    /// </summary>
    public class InheritedGenericControllerOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var methodInfo = context.MethodInfo;
            var controllerType = methodInfo.DeclaringType;

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
            switch (methodInfo.Name)
            {
                case "GetAll":
                    operation.Summary = "获取所有实体";
                    operation.Description = "检索并返回系统中该类型的所有实体记录。";
                    break;

                case "GetById":
                    operation.Summary = "根据ID获取实体";
                    operation.Description = "根据提供的唯一标识符(ID)检索特定实体的详细信息。";
                    if (operation.Parameters.FirstOrDefault(p => p.Name == "id") is OpenApiParameter idParam)
                    {
                        idParam.Description = "实体的唯一标识符";
                    }
                    break;

                case "GetPage":
                    operation.Summary = "分页获取实体";
                    operation.Description = "根据页码和页面大小，分页检索实体记录。";
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
                    operation.Summary = "根据条件分页获取实体";
                    operation.Description = "根据提供的查询条件和分页参数，分页检索实体记录。";
                    // 注意: whereExpression 是FromBody, 通常Swagger UI会处理
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
                    operation.Summary = "创建新实体";
                    operation.Description = "根据提供的数据创建一个新的实体。";
                    var createBodyParam = operation.RequestBody;
                    if (createBodyParam != null)
                    {
                        createBodyParam.Description = "创建实体所需的数据";
                    }
                    break;

                case "Update":
                    operation.Summary = "更新现有实体";
                    operation.Description = "根据提供的ID和更新数据，修改现有实体的信息。";
                    if (operation.Parameters.FirstOrDefault(p => p.Name == "id") is OpenApiParameter updateIdParam)
                    {
                        updateIdParam.Description = "要更新的实体的唯一标识符";
                    }
                    var updateBodyParam = operation.RequestBody;
                    if (updateBodyParam != null)
                    {
                        updateBodyParam.Description = "更新实体所需的数据";
                    }
                    break;

                case "Delete":
                    operation.Summary = "删除实体";
                    operation.Description = "根据提供的ID，从系统中移除指定的实体。";
                    if (operation.Parameters.FirstOrDefault(p => p.Name == "id") is OpenApiParameter deleteIdParam)
                    {
                        deleteIdParam.Description = "要删除的实体的唯一标识符";
                    }
                    break;

                case "BatchDelete":
                    operation.Summary = "批量删除实体";
                    operation.Description = "根据提供的ID列表，批量删除多个实体。";
                    var batchDeleteBodyParam = operation.RequestBody;
                    if (batchDeleteBodyParam != null)
                    {
                        batchDeleteBodyParam.Description = "要删除的实体ID列表";
                    }
                    break;

                // 回收站相关方法
                case "GetRecycleBin":
                    operation.Summary = "获取回收站数据";
                    operation.Description = "检索并返回已软删除的实体记录（回收站数据）。";
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
                    operation.Description = "根据提供的查询条件，检索回收站中的实体记录。";
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
                    operation.Summary = "恢复回收站中的实体";
                    operation.Description = "根据提供的ID，将已软删除的实体恢复到正常状态。";
                    if (operation.Parameters.FirstOrDefault(p => p.Name == "id") is OpenApiParameter restoreIdParam)
                    {
                        restoreIdParam.Description = "要恢复的实体的唯一标识符";
                    }
                    break;

                case "RestoreBatch":
                    operation.Summary = "批量恢复回收站中的实体";
                    operation.Description = "根据提供的条件，批量将回收站中的实体恢复到正常状态。";
                    var restoreBatchBodyParam = operation.RequestBody;
                    if (restoreBatchBodyParam != null)
                    {
                        restoreBatchBodyParam.Description = "用于筛选要恢复实体的条件表达式";
                    }
                    break;

                case "PermanentDelete":
                    operation.Summary = "永久删除回收站中的实体";
                    operation.Description = "根据提供的ID，将已软删除的实体从数据库中永久移除。";
                    if (operation.Parameters.FirstOrDefault(p => p.Name == "id") is OpenApiParameter permDeleteIdParam)
                    {
                        permDeleteIdParam.Description = "要永久删除的实体的唯一标识符";
                    }
                    break;

                case "PermanentDeleteBatch":
                    operation.Summary = "根据条件永久删除回收站中的实体";
                    operation.Description = "根据提供的条件，将回收站中符合条件的实体从数据库中永久移除。";
                    var permDeleteBatchBodyParam = operation.RequestBody;
                    if (permDeleteBatchBodyParam != null)
                    {
                        permDeleteBatchBodyParam.Description = "用于筛选要永久删除实体的条件表达式";
                    }
                    break;
            }
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
}