using Fastdotnet.Plugin.Contracts;

namespace Fastdotnet.Mcp.Tools;

/// <summary>
/// 宿主内置能力的提供者标记接口。
/// <para>
/// 与插件提供的 <see cref="IPluginToolProvider"/> 是同一套契约，但注册位置不同：
/// 内置能力注册在**宿主的 DI 容器**（<c>AddSingleton&lt;IHostToolProvider, Xxx&gt;()</c>），
/// 由框架自身实现，因此不经过插件装载与授权流程。
/// </para>
/// <para>
/// 之所以用独立的标记接口而不是直接注册 <see cref="IPluginToolProvider"/>：
/// 插件作用域是宿主容器的子作用域，若宿主注册了 <c>IPluginToolProvider</c>，
/// 每个插件作用域都会把它解析出来，导致内置能力在每个插件名下重复出现。
/// </para>
/// </summary>
public interface IHostToolProvider : IPluginToolProvider
{
}
