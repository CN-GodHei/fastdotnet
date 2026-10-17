namespace Fastdotnet.Plugin.Contracts
{
    /// <summary>
    /// 插件向 AI / MCP 暴露可调用能力的契约。
    /// <para>
    /// 实现该接口，并在插件的 <see cref="IPlugin.ConfigureServices"/> 中把它注册到插件自己的容器：
    /// <code>
    /// builder.RegisterType&lt;MyToolProvider&gt;().As&lt;IPluginToolProvider&gt;().InstancePerLifetimeScope();
    /// </code>
    /// 注册后，宿主会把这些工具暴露给 AI Agent（MCP 协议），并按
    /// <see cref="PluginToolDescriptor.RequiredPermission"/> 做可见性过滤与调用鉴权。
    /// </para>
    /// <para>
    /// 注意：<b>工具声明是宿主与 Agent 之间的安全边界</b>——请如实填写
    /// <see cref="PluginToolDescriptor.RequiredPermission"/> 与
    /// <see cref="PluginToolDescriptor.IsDestructive"/>，不要用反射自动推断。
    /// </para>
    /// </summary>
    public interface IPluginToolProvider
    {
        /// <summary>声明本插件提供的全部工具（纯元数据，不应有副作用）</summary>
        IEnumerable<PluginToolDescriptor> GetTools();

        /// <summary>执行一次工具调用</summary>
        Task<ToolInvocationResult> InvokeAsync(
            string toolName,
            ToolInvocationContext context,
            CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// 工具声明。会被转换为 MCP 工具的 name / description / inputSchema。
    /// </summary>
    public sealed class PluginToolDescriptor
    {
        /// <summary>插件内唯一名称，建议 snake_case，如 query_order_status</summary>
        public string Name { get; init; } = string.Empty;

        /// <summary>人类可读标题</summary>
        public string Title { get; init; } = string.Empty;

        /// <summary>
        /// 给 LLM 看的说明。<b>这是决定 Agent 调用准确率的关键字段</b>：
        /// 应写清「做什么、什么时候用、参数含义、返回什么、有什么限制」。
        /// </summary>
        public string Description { get; init; } = string.Empty;

        /// <summary>参数声明</summary>
        public IReadOnlyList<PluginToolParameter> Parameters { get; init; } = Array.Empty<PluginToolParameter>();

        /// <summary>
        /// 调用所需 RBAC 权限码（对应宿主权限体系中的 Code）。
        /// null 表示「登录即可调用」。
        /// </summary>
        public string? RequiredPermission { get; init; }

        /// <summary>true = 有副作用的写操作，生产环境应走人工确认后才允许调用</summary>
        public bool IsDestructive { get; init; }

        /// <summary>单次调用超时（秒）；null 使用宿主默认值（30s）</summary>
        public int? TimeoutSeconds { get; init; }
    }

    /// <summary>工具参数声明</summary>
    public sealed class PluginToolParameter
    {
        public string Name { get; init; } = string.Empty;

        public ToolParameterType Type { get; init; } = ToolParameterType.String;

        public bool Required { get; init; }

        public string? Description { get; init; }

        /// <summary>枚举约束（会生成 JSON Schema 的 enum）</summary>
        public IReadOnlyList<string>? Enum { get; init; }

        /// <summary>默认值（会写入 JSON Schema 的 default）</summary>
        public object? Default { get; init; }
    }

    /// <summary>参数类型（映射为 JSON Schema 类型）</summary>
    public enum ToolParameterType
    {
        String,
        Integer,
        Number,
        Boolean,
        Array,
        Object
    }

    /// <summary>
    /// 调用上下文：由宿主填充，插件只读。
    /// </summary>
    public sealed class ToolInvocationContext
    {
        public string PluginId { get; init; } = string.Empty;

        public string ToolName { get; init; } = string.Empty;

        /// <summary>调用者用户 ID（来自宿主 JWT）</summary>
        public string? UserId { get; init; }

        /// <summary>调用者用户名</summary>
        public string? UserName { get; init; }

        /// <summary>调用者类别：Admin / App</summary>
        public string UserType { get; init; } = "Admin";

        /// <summary>是否超级管理员（超管跳过权限校验）</summary>
        public bool IsSuperAdmin { get; init; }

        /// <summary>租户标识（多租户隔离用）</summary>
        public string? TenantId { get; init; }

        /// <summary>链路追踪 ID，与宿主 RequestId 打通</summary>
        public string? RequestId { get; init; }

        /// <summary>已按 inputSchema 解析后的参数</summary>
        public IReadOnlyDictionary<string, object?> Arguments { get; init; } =
            new Dictionary<string, object?>();

        /// <summary>
        /// 插件自己的 Autofac 生命周期作用域。
        /// 插件内部服务通过 <c>context.PluginServices.Resolve&lt;T&gt;()</c> 解析，
        /// 不要访问宿主的全局容器。
        /// <para>宿主内置能力（非插件）调用时为 <c>null</c>——这类提供者通过构造函数注入依赖。</para>
        /// </summary>
        public ILifetimeScope? PluginServices { get; init; }
    }

    /// <summary>工具调用结果</summary>
    public sealed class ToolInvocationResult
    {
        public bool Success { get; init; }

        /// <summary>给 LLM 消费的文本结果（Markdown 友好）</summary>
        public string? Content { get; init; }

        /// <summary>结构化结果（可选）</summary>
        public object? StructuredContent { get; init; }

        /// <summary>机器可读错误码，如 PERMISSION_DENIED / INVALID_ARGUMENT / TIMEOUT</summary>
        public string? ErrorCode { get; init; }

        public string? ErrorMessage { get; init; }

        public static ToolInvocationResult Ok(string content, object? structured = null)
            => new() { Success = true, Content = content, StructuredContent = structured };

        public static ToolInvocationResult Fail(string code, string message)
            => new() { Success = false, ErrorCode = code, ErrorMessage = message };
    }
}
