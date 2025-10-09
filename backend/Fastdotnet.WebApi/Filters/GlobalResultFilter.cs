using Fastdotnet.Core.Attributes;
using Fastdotnet.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace Fastdotnet.WebApi.Filters
{
    /// <summary>
    /// 全局结果过滤器，用于统一API返回格式（仅处理非异常结果）
    /// 异常应由全局异常中间件统一处理
    /// </summary>
    public class GlobalResultFilter : IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            // 1. 检查是否跳过全局结果处理
            if (context.ActionDescriptor.EndpointMetadata.Any(em => em is SkipGlobalResultAttribute))
            {
                return;
            }

            // 2. 只处理非异常结果（异常应已被中间件捕获）
            if (context.Result is ObjectResult objResult)
            {
                // 如果已经是 ApiResult<T>，跳过包装
                if (IsApiResult(objResult.Value))
                {
                    return;
                }

                // 处理 PageResult<T>
                if (objResult.Value != null && IsPageResult(objResult.Value, out var pageResultInfo))
                {
                    // 构造包含分页信息的匿名对象
                    var pageData = new
                    {
                        Data = pageResultInfo.Data,
                        TotalCount = pageResultInfo.TotalCount,
                        PageIndex = pageResultInfo.PageIndex,
                        PageSize = pageResultInfo.PageSize,
                        TotalPages = pageResultInfo.TotalPages
                    };

                    var wrappedResult = ApiResult<object>.Success(pageData);
                    context.Result = new ObjectResult(wrappedResult)
                    {
                        StatusCode = objResult.StatusCode
                    };
                    return;
                }

                // 普通对象或 null
                var finalResult = objResult.Value == null
                    ? ApiResult<object>.Success(null)
                    : ApiResult<object>.Success(objResult.Value);

                context.Result = new ObjectResult(finalResult)
                {
                    StatusCode = objResult.StatusCode // 保留原始状态码（如 201 Created）
                };
            }
            else if (context.Result is EmptyResult)
            {
                context.Result = new ObjectResult(ApiResult<object>.Success(null))
                {
                    StatusCode = 200
                };
            }
            else if (context.Result is StatusCodeResult statusCodeResult)
            {
                // 保留状态码，但包装消息（可选）
                var message = $"请求完成，状态码: {statusCodeResult.StatusCode}";
                context.Result = new ObjectResult(ApiResult<object>.Success(message))
                {
                    StatusCode = statusCodeResult.StatusCode
                };
            }
            else if (context.Result is ContentResult contentResult)
            {
                context.Result = new ObjectResult(ApiResult<string>.Success(contentResult.Content))
                {
                    StatusCode = contentResult.StatusCode
                };
            }
            // 其他 Result 类型（如 RedirectResult）通常不用于 API，可忽略
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            // 无需处理
        }

        // 判断是否已经是 ApiResult<T>
        private static bool IsApiResult(object value)
        {
            if (value == null) return false;
            var type = value.GetType();
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ApiResult<>);
        }

        // 尝试识别 PageResult<T> 并提取属性
        private static bool IsPageResult(object value, out (object Data, long TotalCount, int PageIndex, int PageSize, int TotalPages) info)
        {
            info = default;
            if (value == null) return false;

            var type = value.GetType();
            // 检查是否是 Fastdotnet.Core.Models.ApiResult.PageResult<T>
            if (!type.IsGenericType || type.DeclaringType?.Name != "ApiResult" || type.Name != "PageResult")
                return false;

            // 使用属性名反射（比硬编码类型更安全）
            var dataProp = type.GetProperty("Data");
            var totalCountProp = type.GetProperty("TotalCount");
            var pageIndexProp = type.GetProperty("PageIndex");
            var pageSizeProp = type.GetProperty("PageSize");
            var totalPagesProp = type.GetProperty("TotalPages");

            if (dataProp == null || totalCountProp == null || pageIndexProp == null ||
                pageSizeProp == null || totalPagesProp == null)
                return false;

            info = (
                Data: dataProp.GetValue(value),
                TotalCount: (long)totalCountProp.GetValue(value)!,
                PageIndex: (int)pageIndexProp.GetValue(value)!,
                PageSize: (int)pageSizeProp.GetValue(value)!,
                TotalPages: (int)totalPagesProp.GetValue(value)!
            );
            return true;
        }
    }
}