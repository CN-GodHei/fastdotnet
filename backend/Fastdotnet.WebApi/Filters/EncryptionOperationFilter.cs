using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Fastdotnet.WebApi.Filters
{
    /// <summary>
    /// 将加密相关特性信息添加到Swagger文档的过滤器
    /// </summary>
    public class EncryptionOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // 使用帮助类检查是否启用了加密
            var isRequestEncrypted = Core.Attributes.EncryptionAttributeHelper.IsRequestEncryptionEnabled(context.MethodInfo);
            var isResponseEncrypted = Core.Attributes.EncryptionAttributeHelper.IsResponseEncryptionEnabled(context.MethodInfo);

            // 如果有任一加密特性，则添加扩展信息和 UI 显示信息
            if (isRequestEncrypted || isResponseEncrypted)
            {
                var encryptionInfo = new OpenApiObject();
                
                // 保存原始摘要和描述，用于后续追加加密信息
                var originalSummary = operation.Summary ?? "";
                var originalDescription = operation.Description ?? "";

                if (isRequestEncrypted)
                {
                    var requestEncryption = new OpenApiObject
                    {
                        ["algorithm"] = new OpenApiString(Core.Attributes.EncryptionAttributeHelper.GetRequestEncryptionAlgorithm(context.MethodInfo)),
                        ["keyIdentifier"] = new OpenApiString(Core.Attributes.EncryptionAttributeHelper.GetRequestEncryptionKeyIdentifier(context.MethodInfo))
                    };
                    encryptionInfo["request"] = requestEncryption;
                    
                    // 在摘要中添加请求加密提示
                    operation.Summary = originalSummary + (string.IsNullOrEmpty(originalSummary) ? "" : " ") + "🔐[请求加密]";
                    
                    // 添加详细描述
                    var requestEncryptionDesc = $"\n\n**请求加密**: 该接口的请求参数需要使用 {Core.Attributes.EncryptionAttributeHelper.GetRequestEncryptionAlgorithm(context.MethodInfo)} 算法进行加密。" +
                                              (string.IsNullOrEmpty(Core.Attributes.EncryptionAttributeHelper.GetRequestEncryptionKeyIdentifier(context.MethodInfo)) 
                                               ? "" 
                                               : $"密钥标识: {Core.Attributes.EncryptionAttributeHelper.GetRequestEncryptionKeyIdentifier(context.MethodInfo)}");
                    operation.Description = originalDescription + requestEncryptionDesc;
                }

                if (isResponseEncrypted)
                {
                    var responseEncryption = new OpenApiObject
                    {
                        ["algorithm"] = new OpenApiString(Core.Attributes.EncryptionAttributeHelper.GetResponseEncryptionAlgorithm(context.MethodInfo)),
                        ["keyIdentifier"] = new OpenApiString(Core.Attributes.EncryptionAttributeHelper.GetResponseEncryptionKeyIdentifier(context.MethodInfo))
                    };
                    encryptionInfo["response"] = responseEncryption;
                    
                    // 在摘要中添加响应加密提示
                    var summaryToAddTo = isRequestEncrypted ? operation.Summary : originalSummary;
                    operation.Summary = summaryToAddTo + (string.IsNullOrEmpty(summaryToAddTo) ? "" : " ") + "🔐[响应加密]";
                    
                    // 添加详细描述
                    var responseEncryptionDesc = $"\n\n**响应加密**: 该接口的响应数据使用 {Core.Attributes.EncryptionAttributeHelper.GetResponseEncryptionAlgorithm(context.MethodInfo)} 算法进行加密。" +
                                               (string.IsNullOrEmpty(Core.Attributes.EncryptionAttributeHelper.GetResponseEncryptionKeyIdentifier(context.MethodInfo)) 
                                                ? "" 
                                                : $"密钥标识: {Core.Attributes.EncryptionAttributeHelper.GetResponseEncryptionKeyIdentifier(context.MethodInfo)}");
                    operation.Description = operation.Description + responseEncryptionDesc;
                }

                // 将加密信息添加到扩展中（用于前端代码生成）
                operation.Extensions["x-encryption-info"] = encryptionInfo;
            }
        }
    }
}