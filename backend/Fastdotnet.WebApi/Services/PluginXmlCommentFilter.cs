using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace Fastdotnet.WebApi.Services;

/// <summary>
/// 插件 XML 注释过滤器，在运行时将插件的 XML 注释应用到 Swagger 文档
/// 支持启动时和运行时动态注册的插件
/// </summary>
public class PluginXmlCommentFilter : IOperationFilter
{
    // 插件 XML 文档缓存: pluginId (lower) -> XDocument
    private static readonly ConcurrentDictionary<string, XDocument> _pluginXmlDocs = new();
    // 程序集名到插件 ID 的映射: assemblyName -> pluginId (lower)
    private static readonly ConcurrentDictionary<string, string> _assemblyToPluginId = new();

    /// <summary>
    /// 注册插件的 XML 注释文件
    /// </summary>
    public static void AddPluginXml(string pluginId, string assemblyName, string xmlFilePath)
    {
        try
        {
            if (!File.Exists(xmlFilePath))
                return;

            var key = pluginId.ToLower();
            _pluginXmlDocs[key] = XDocument.Load(xmlFilePath);
            _assemblyToPluginId[assemblyName] = key;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"加载插件 XML 注释失败 [{pluginId}]: {ex.Message}");
        }
    }

    /// <summary>
    /// 批量注册插件的 XML 注释文件
    /// </summary>
    public static void AddPluginXmlRange(IEnumerable<(string pluginId, string assemblyName, string xmlFilePath)> entries)
    {
        foreach (var (pluginId, assemblyName, xmlFilePath) in entries)
        {
            AddPluginXml(pluginId, assemblyName, xmlFilePath);
        }
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var methodInfo = context.MethodInfo;
        var declaringType = methodInfo.DeclaringType;
        if (declaringType == null) return;

        var assemblyName = declaringType.Assembly.GetName().Name;
        if (assemblyName == null || !_assemblyToPluginId.TryGetValue(assemblyName, out var pluginId))
            return;

        if (!_pluginXmlDocs.TryGetValue(pluginId, out var xmlDoc))
            return;

        // 构建 XML member 名称并应用注释
        var memberName = BuildMemberName(methodInfo);
        if (string.IsNullOrEmpty(memberName))
            return;

        ApplyMemberComments(operation, xmlDoc, memberName);

        // 尝试为参数应用注释
        ApplyParameterComments(operation, context, xmlDoc, memberName);
    }

    private static string BuildMemberName(MethodInfo method)
    {
        var builder = new StringBuilder("M:");

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

    private static void AppendFullTypeName(StringBuilder builder, Type type)
    {
        if (type.IsGenericType)
        {
            var fullName = type.GetGenericTypeDefinition().FullName;
            builder.Append(fullName!.Replace('+', '.').Replace('`', '\''));
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

    private static void ApplyMemberComments(OpenApiOperation operation, XDocument xmlDoc, string memberName)
    {
        var memberElement = xmlDoc.Root?
            .Element("members")?
            .Elements("member")
            .FirstOrDefault(m => m.Attribute("name")?.Value == memberName);

        if (memberElement == null) return;

        // 应用 <summary>
        var summaryElement = memberElement.Element("summary");
        if (summaryElement != null && string.IsNullOrWhiteSpace(operation.Summary))
        {
            operation.Summary = summaryElement.Value.Trim();
        }

        // 应用 <remarks>
        var remarksElement = memberElement.Element("remarks");
        if (remarksElement != null && string.IsNullOrWhiteSpace(operation.Description))
        {
            operation.Description = remarksElement.Value.Trim();
        }
    }

    private static void ApplyParameterComments(OpenApiOperation operation, OperationFilterContext context,
        XDocument xmlDoc, string memberName)
    {
        var memberElement = xmlDoc.Root?
            .Element("members")?
            .Elements("member")
            .FirstOrDefault(m => m.Attribute("name")?.Value == memberName);

        if (memberElement == null) return;

        var parameters = context.MethodInfo.GetParameters();
        foreach (var param in parameters)
        {
            var paramElement = memberElement.Elements("param")
                .FirstOrDefault(p => p.Attribute("name")?.Value == param.Name);
            if (paramElement == null) continue;

            var paramDoc = paramElement.Value.Trim();
            if (string.IsNullOrEmpty(paramDoc)) continue;

            // 查找并更新对应的 OpenAPI 参数
            var openApiParam = operation.Parameters?.FirstOrDefault(p =>
                p.Name.Equals(param.Name, StringComparison.OrdinalIgnoreCase));
            if (openApiParam != null && string.IsNullOrWhiteSpace(openApiParam.Description))
            {
                openApiParam.Description = paramDoc;
            }
        }
    }
}
