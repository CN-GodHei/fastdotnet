using Fastdotnet.Core.Attributes;
using Fastdotnet.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Reflection;

namespace Fastdotnet.WebApi.Filters
{
    /// <summary>
    /// 全局结果过滤器，用于统一API返回格式
    /// </summary>
    public class GlobalResultFilter : IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            // 检查是否标记了跳过全局结果处理的特性
            if (context.ActionDescriptor.EndpointMetadata.Any(em => em.GetType() == typeof(SkipGlobalResultAttribute)))
            {
                return;
            }

            // 对不同的结果类型进行统一包装
            switch (context.Result)
            {
                case ObjectResult objResult:
                    // 如果已经是ApiResult类型，则不重复包装
                    if (objResult.Value != null && objResult.Value.GetType().IsGenericType && 
                        objResult.Value.GetType().GetGenericTypeDefinition() == typeof(ApiResult<>))
                    {
                        break;
                    }

                    // 检查是否为PageResult类型
                    if (objResult.Value != null && objResult.Value.GetType().FullName.StartsWith("Fastdotnet.Core.Models.ApiResult+PageResult"))
                    {
                        // 获取PageResult实例的属性值
                        var pageResult = objResult.Value;
                        var dataType = pageResult.GetType().GetGenericArguments()[0];
                        
                        // 通过反射获取PageResult的属性值
                        var dataProperty = pageResult.GetType().GetProperty("Data");
                        var totalCountProperty = pageResult.GetType().GetProperty("TotalCount");
                        var pageIndexProperty = pageResult.GetType().GetProperty("PageIndex");
                        var pageSizeProperty = pageResult.GetType().GetProperty("PageSize");
                        var totalPagesProperty = pageResult.GetType().GetProperty("TotalPages");
                        
                        var dataValue = dataProperty.GetValue(pageResult);
                        var totalCountValue = totalCountProperty.GetValue(pageResult);
                        var pageIndexValue = pageIndexProperty.GetValue(pageResult);
                        var pageSizeValue = pageSizeProperty.GetValue(pageResult);
                        var totalPagesValue = totalPagesProperty.GetValue(pageResult);
                        
                        // 创建一个匿名对象包含分页信息和数据
                        var resultObject = new {
                            Data = dataValue,
                            TotalCount = totalCountValue,
                            PageIndex = pageIndexValue,
                            PageSize = pageSizeValue,
                            TotalPages = totalPagesValue
                        };
                        
                        // 包装为ApiResult
                        var ApiResultType = typeof(ApiResult<>).MakeGenericType(resultObject.GetType());
                        var successMethod = ApiResultType.GetMethod("Success", new[] { resultObject.GetType() });
                        var wrappedResult = successMethod?.Invoke(null, new[] { resultObject });
                        
                        context.Result = new ObjectResult(wrappedResult)
                        {
                            StatusCode = objResult.StatusCode
                        };
                        break;
                    }
                    // 包装普通对象结果
                    else if (objResult.Value != null)
                    {
                        var resultType = objResult.Value.GetType();
                        var ApiResultType = typeof(ApiResult<>).MakeGenericType(resultType);
                        var successMethod = ApiResultType.GetMethod("Success", new[] { resultType });
                        var wrappedResult = successMethod?.Invoke(null, new[] { objResult.Value });
                        context.Result = new ObjectResult(wrappedResult)
                        {
                            StatusCode = objResult.StatusCode
                        };
                    }
                    else
                    {
                        // 处理返回null的情况
                        var ApiResultType = typeof(ApiResult<object>);
                        var successMethod = ApiResultType.GetMethod("Success", new[] { typeof(object) });
                        var wrappedResult = successMethod?.Invoke(null, new object[] { null });
                        context.Result = new ObjectResult(wrappedResult)
                        {
                            StatusCode = objResult.StatusCode
                        };
                    }
                    break;

                case EmptyResult emptyResult:
                    // 包装空结果
                    context.Result = new ObjectResult(new ApiResult<object>() { Data=null})
                    {
                        StatusCode = 200
                    };
                    break;

                case StatusCodeResult statusCodeResult:
                    // 包装状态码结果
                    context.Result = new ObjectResult(new ApiResult<object>() { Data = $"请求失败，状态码: {statusCodeResult.StatusCode}" })
                    {
                        StatusCode = statusCodeResult.StatusCode
                    };
                    break;

                case ContentResult contentResult:
                    // 包装内容结果
                    context.Result = new ObjectResult(new ApiResult<string>() { Data= contentResult.Content })
                    {
                        StatusCode = contentResult.StatusCode
                    };
                    break;
            }
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            // 执行后处理（如果需要）
        }
    }
}