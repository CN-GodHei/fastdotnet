using Fastdotnet.Core.Dtos;
using Fastdotnet.Core.Attributes;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Cryptography;
using System.Text;

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
                // 如果已经是 ApiResult<T>，直接包装
                if (objResult.Value != null && IsApiResult(objResult.Value))
                {
                    // 保持原样，后续统一处理加密
                }
                // 处理 PageResult<T>
                else if (objResult.Value != null && IsPageResult(objResult.Value, out var pageResultInfo))
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
                }
                // 普通对象或 null
                else
                {
                    var finalResult = objResult.Value == null
                        ? ApiResult<object>.Success(null!)
                        : ApiResult<object>.Success(objResult.Value);

                    context.Result = new ObjectResult(finalResult)
                    {
                        StatusCode = objResult.StatusCode // 保留原始状态码（如 201 Created）
                    };
                }

                // 统一检查是否需要加密（此时已经是 ApiResult<T>）
                EncryptApiResultIfNeeded(context, (ObjectResult)context.Result);
            }
            else if (context.Result is EmptyResult)
            {
                var result = new ObjectResult(ApiResult<object>.Success(null!))
                {
                    StatusCode = 200
                };
                context.Result = result;
                EncryptApiResultIfNeeded(context, result);
            }
            else if (context.Result is StatusCodeResult statusCodeResult)
            {
                // 保留状态码，设置消息
                var message = $"请求完成，状态码: {statusCodeResult.StatusCode}";
                var result = new ObjectResult(ApiResult.FromCode(statusCodeResult.StatusCode, message))
                {
                    StatusCode = statusCodeResult.StatusCode
                };
                context.Result = result;
                EncryptApiResultIfNeeded(context, result);
            }
            else if (context.Result is ContentResult contentResult)
            {
                var result = new ObjectResult(ApiResult<string>.Success(contentResult.Content ?? string.Empty))
                {
                    StatusCode = contentResult.StatusCode
                };
                context.Result = result;
                EncryptApiResultIfNeeded(context, result);
            }
            // 其他 Result 类型（如 RedirectResult）通常不用于 API，可忽略
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            // 无需处理
        }

        // 判断是否已经是 ApiResult 或 ApiResult<T>
        private static bool IsApiResult(object value)
        {
            if (value == null) return false;
            var type = value.GetType();
            
            // 检查是否是 ApiResult<T>
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ApiResult<>))
            {
                return true;
            }
            
            // 检查是否是非泛型的 ApiResult
            if (type == typeof(ApiResult))
            {
                return true;
            }
            
            return false;
        }

        // 尝试识别 PageResult<T> 并提取属性
        private static bool IsPageResult(object value, out (object? Data, long TotalCount, int PageIndex, int PageSize, int TotalPages) info)
        {
            info = default;
            if (value == null) return false;

            var type = value.GetType();
            // 检查是否是 Fastdotnet.Core.Dtos.ApiResult.PageResult<T>
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
                Data: (object?)dataProp.GetValue(value),
                TotalCount: (long)totalCountProp.GetValue(value)!,
                PageIndex: (int)pageIndexProp.GetValue(value)!,
                PageSize: (int)pageSizeProp.GetValue(value)!,
                TotalPages: (int)totalPagesProp.GetValue(value)!
            );
            return true;
        }

        /// <summary>
        /// 如果标记了 [EncryptResponse]，则对 ApiResult 的 Data 字段进行加密
        /// </summary>
        private void EncryptApiResultIfNeeded(ResultExecutingContext context, ObjectResult objResult)
        {
            // 检查是否有 [EncryptResponse] 标记（复用 EncryptionMiddleware 的逻辑）
            var actionDescriptor = context.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
            if (actionDescriptor == null) return;
            
            var methodInfo = actionDescriptor.MethodInfo;
            var controllerTypeInfo = actionDescriptor.ControllerTypeInfo;
            
            // 使用 EncryptionAttributeHelper 检查是否需要加密
            if (!Fastdotnet.Core.Attributes.EncryptionAttributeHelper.IsResponseEncryptionEnabled(methodInfo) &&
                !Fastdotnet.Core.Attributes.EncryptionAttributeHelper.IsResponseEncryptionEnabled(controllerTypeInfo))
            {
                return;
            }
            
            // 获取加密算法（默认 AES-256-CBC）
            var algorithm = Fastdotnet.Core.Attributes.EncryptionAttributeHelper.GetResponseEncryptionAlgorithm(methodInfo) ??
                           Fastdotnet.Core.Attributes.EncryptionAttributeHelper.GetResponseEncryptionAlgorithm(controllerTypeInfo) ??
                           "AES-256-CBC";
            
            // 只对 ApiResult<T> 进行加密
            if (objResult.Value == null) return;
            var apiResultType = objResult.Value.GetType();
            if (apiResultType.IsGenericType && apiResultType.GetGenericTypeDefinition() == typeof(ApiResult<>))
            {
                try
                {
                    // 使用反射获取 Data 属性
                    var dataProp = apiResultType.GetProperty("Data");
                    var dataValue = dataProp?.GetValue(objResult.Value);
                    
                    if (dataValue == null) return;
                    
                    // 将 Data 序列化为 JSON
                    var dataJson = System.Text.Json.JsonSerializer.Serialize(dataValue);
                    
                    // 使用 AES-256-CBC 加密
                    using var aes = Aes.Create();
                    aes.KeySize = 256;
                    aes.GenerateKey();
                    aes.GenerateIV();
                    
                    byte[] plainBytes = Encoding.UTF8.GetBytes(dataJson);
                    byte[] encryptedData;
                    
                    using (var encryptor = aes.CreateEncryptor())
                    {
                        encryptedData = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                    }
                    
                    //// 将加密后的数据和密钥信息封装
                    //var encryptedPayload = new
                    //{
                    //    encryptedData = Convert.ToBase64String(encryptedData),
                    //    // key = Convert.ToBase64String(aes.Key),
                    //    // iv = Convert.ToBase64String(aes.IV),
                    //    // algorithm = "AES-256-CBC"
                    //};
                    
                    // 替换 Data 为加密后的对象
                    dataProp?.SetValue(objResult.Value, Convert.ToBase64String(encryptedData));
                    
                    // 设置响应头，传递密钥信息（HTTPS 环境下安全）
                    context.HttpContext.Response.Headers.Append("X-Encryption-Key", Convert.ToBase64String(aes.Key));
                    context.HttpContext.Response.Headers.Append("X-Encryption-IV", Convert.ToBase64String(aes.IV));
                    context.HttpContext.Response.Headers.Append("X-Encryption-Algorithm", "AES-256-CBC");
                }
                catch
                {
                    // 加密失败时保持原样
                }
            }
        }
    }
}