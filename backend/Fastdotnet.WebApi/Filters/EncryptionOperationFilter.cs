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
                    var algorithm = Core.Attributes.EncryptionAttributeHelper.GetRequestEncryptionAlgorithm(context.MethodInfo);
                    var keyIdentifier = Core.Attributes.EncryptionAttributeHelper.GetRequestEncryptionKeyIdentifier(context.MethodInfo);
                    
                    // 判断是否为混合加密（算法名称包含 HYBRID 或 AES）
                    var isHybrid = algorithm.ToUpper().Contains("HYBRID") || algorithm.ToUpper().Contains("AES");
                    var displayAlgorithm = isHybrid ? "HYBRID" : algorithm;
                    
                    var requestEncryption = new OpenApiObject
                    {
                        ["algorithm"] = new OpenApiString(displayAlgorithm),
                        ["originalAlgorithm"] = new OpenApiString(algorithm),
                        ["keyIdentifier"] = new OpenApiString(keyIdentifier),
                        ["isHybrid"] = new OpenApiBoolean(isHybrid)
                    };
                    encryptionInfo["request"] = requestEncryption;
                    
                    // 在摘要中添加请求加密提示
                    var encryptionTip = isHybrid ? "🔐[请求加密-混合]" : "🔐[请求加密]";
                    operation.Summary = originalSummary + (string.IsNullOrEmpty(originalSummary) ? "" : " ") + encryptionTip;
                    
                    // 添加详细描述
                    var algorithmDesc = isHybrid 
                        ? "RSA + AES 混合加密" 
                        : $"{algorithm} 算法";
                    var requestEncryptionDesc = $"\n\n**请求加密**: 该接口的请求参数需要使用 {algorithmDesc} 进行加密。" +
                                              (string.IsNullOrEmpty(keyIdentifier) 
                                               ? "" 
                                               : $"密钥标识: {keyIdentifier}");
                    operation.Description = originalDescription + requestEncryptionDesc;
                }

                if (isResponseEncrypted)
                {
                    var algorithm = Core.Attributes.EncryptionAttributeHelper.GetResponseEncryptionAlgorithm(context.MethodInfo);
                    var keyIdentifier = Core.Attributes.EncryptionAttributeHelper.GetResponseEncryptionKeyIdentifier(context.MethodInfo);
                    
                    // 判断是否为混合加密
                    var isHybrid = algorithm.ToUpper().Contains("HYBRID") || algorithm.ToUpper().Contains("AES");
                    var displayAlgorithm = isHybrid ? "HYBRID" : algorithm;
                    
                    var responseEncryption = new OpenApiObject
                    {
                        ["algorithm"] = new OpenApiString(displayAlgorithm),
                        ["originalAlgorithm"] = new OpenApiString(algorithm),
                        ["keyIdentifier"] = new OpenApiString(keyIdentifier),
                        ["isHybrid"] = new OpenApiBoolean(isHybrid)
                    };
                    encryptionInfo["response"] = responseEncryption;
                    
                    // 在摘要中添加响应加密提示
                    var summaryToAddTo = isRequestEncrypted ? operation.Summary : originalSummary;
                    var encryptionTip = isHybrid ? "🔐[响应加密-混合]" : "🔐[响应加密]";
                    operation.Summary = summaryToAddTo + (string.IsNullOrEmpty(summaryToAddTo) ? "" : " ") + encryptionTip;
                    
                    // 添加详细描述
                    var algorithmDesc = isHybrid 
                        ? "RSA + AES 混合加密" 
                        : $"{algorithm} 算法";
                    var responseEncryptionDesc = $"\n\n**响应加密**: 该接口的响应数据使用 {algorithmDesc} 进行加密。" +
                                               (string.IsNullOrEmpty(keyIdentifier) 
                                                ? "" 
                                                : $"密钥标识: {keyIdentifier}");
                    operation.Description = operation.Description + responseEncryptionDesc;
                }

                // 将加密信息添加到扩展中（用于前端代码生成）
                operation.Extensions["x-encryption-info"] = encryptionInfo;
            }
        }
    }
}