using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Fastdotnet.WebApi.Filters
{
    /// <summary>
    /// 为API操作生成稳定的操作ID，确保前端生成的函数名不受路由前缀变化影响
    /// </summary>
    public class StableOperationNameFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // 生成稳定的操作ID
            var stableOperationId = GenerateStableOperationId(context);
            
            // 将稳定操作ID添加到扩展字段（供openapi2ts使用）
            operation.Extensions["x-stable-operation-id"] = new OpenApiString(stableOperationId);
            
            // 同时设置 operationId，openapi2ts 会优先使用这个值
            operation.OperationId = stableOperationId;
        }
        
        /// <summary>
        /// 生成稳定的操作ID
        /// 格式: {controller}-{method}（全小写，连字符分隔）
        /// 例如: unifiedpay-pay, captcha-generate, user-deletebyid
        /// 设计理念：
        /// 1. 不包含HTTP方法，因为GET→POST等变更是常见重构
        /// 2. 移除Async后缀，因为这是C#实现细节
        /// 3. 统一使用小写+连字符，避免大小写变化影响
        /// </summary>
        private string GenerateStableOperationId(OperationFilterContext context)
        {
            // 获取控制器名称（去掉Controller后缀）
            var controllerType = context.MethodInfo.DeclaringType;
            if (controllerType == null)
                return "unknown";
                
            var controllerName = controllerType.Name;
            if (controllerName.EndsWith("Controller"))
            {
                controllerName = controllerName.Substring(0, controllerName.Length - "Controller".Length);
            }
            
            // 获取方法名称
            var methodName = context.MethodInfo.Name;
            
            // 移除Async后缀（如果存在）
            if (methodName.EndsWith("Async"))
            {
                methodName = methodName.Substring(0, methodName.Length - "Async".Length);
            }
            
            // 转换为 kebab-case（小写+连字符）
            var controllerKebab = ToKebabCase(controllerName);
            var methodKebab = ToKebabCase(methodName);
            
            // 组合成稳定的操作ID
            return $"{controllerKebab}-{methodKebab}";
        }
        
        /// <summary>
        /// 将 PascalCase 或 camelCase 转换为 kebab-case
        /// 例如: UnifiedPay -> unified-pay, getUserName -> get-user-name
        /// </summary>
        private string ToKebabCase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
                
            var result = new System.Text.StringBuilder();
            
            for (int i = 0; i < input.Length; i++)
            {
                var c = input[i];
                
                // 如果是大写字母且不是第一个字符
                if (char.IsUpper(c) && i > 0)
                {
                    // 在前一个字符是小写或数字时添加连字符
                    var prevChar = input[i - 1];
                    if (char.IsLower(prevChar) || char.IsDigit(prevChar))
                    {
                        result.Append('-');
                    }
                    // 处理连续大写字母后的一个小写字母（如XMLParser -> xml-parser）
                    else if (i + 1 < input.Length && char.IsLower(input[i + 1]))
                    {
                        result.Append('-');
                    }
                }
                
                result.Append(char.ToLower(c));
            }
            
            return result.ToString();
        }
    }
    
    /// <summary>
    /// 字符串扩展方法
    /// </summary>
    internal static class StringExtensions
    {
        public static string FirstCharToUpper(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
                
            return input.First().ToString().ToUpper() + input.Substring(1);
        }
    }
}
